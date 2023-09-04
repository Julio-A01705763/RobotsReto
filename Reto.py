from mesa import Agent, Model 
from mesa.space import MultiGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector
import matplotlib
import matplotlib.pyplot as plt
import matplotlib.animation as animation
plt.rcParams["animation.html"] = "jshtml"
matplotlib.rcParams['animation.embed_limit'] = 2**128
import numpy as np
import pandas as pd
import time
import datetime
from matplotlib.colors import ListedColormap
from matplotlib.colors import BoundaryNorm, ListedColormap
import random

class Robot(Agent):
    def __init__(self, id, model, pos, trash_bin, target_position):
        super().__init__(id, model)
        self.id = id
        self.pos = pos
        self.trash_capacity = 5
        self.current_trash = 0
        self.trash_bin = trash_bin
        self.original_cell_value = self.model.grid_data[pos[1]][pos[0]]
        self.mapping = True
        self.go_zone = True
        self.target_position = target_position
        self.previous_pos = None
        self.i = 0

    def is_valid_position(self, pos):
        return 0 <= pos[0] < len(self.model.grid_data[0]) and 0 <= pos[1] < len(self.model.grid_data)

    def get_possible_steps(self, current_pos):
        possible_steps = self.model.grid.get_neighborhood(
            current_pos,
            moore=True,
            include_center=False,
        )
        
        return possible_steps
    
    def get_filtered_possible_steps(self, current_pos, radius=1):
        disallowed_values = {101, 105, 106, 107, 108, 109}

        possible_steps = self.model.grid.get_neighborhood(
            current_pos,
            moore=True,
            include_center=False,
            radius=radius
        )
        
        return [pos for pos in possible_steps if self.is_valid_position(pos) and self.model.grid_data[pos[1]][pos[0]] not in disallowed_values]

    def move_to_position(self, next_position):
        self.model.grid_data[self.pos[1]][self.pos[0]] = self.original_cell_value
        self.original_cell_value = self.model.grid_data[next_position[1]][next_position[0]]
        self.model.grid.move_agent(self, next_position)
        self.model.grid_data[next_position[1]][next_position[0]] = self.unique_id

    def step(self):
        if self.mapping:
            disallowed_values = {101, 105, 106, 107, 108, 109}

            if self.go_zone:
                # Si aún no hemos llegado a la posición objetivo o estamos lo suficientemente cerca
                distance_to_target = abs(self.pos[0] - self.target_position[0]) + abs(self.pos[1] - self.target_position[1])
                
                if distance_to_target > 2:  # Asumo que quieres que esté dentro de un radio de 2 para considerarlo "cerca".
                    # Moverse hacia la posición objetivo
                    self.map_colmena(self.get_possible_steps(self.pos))
                    possible_steps = self.get_filtered_possible_steps(self.pos)
                    
                    if not possible_steps:
                        return
                    
                    next_position = min(possible_steps, key=lambda pos: abs(pos[0] - self.target_position[0]) + abs(pos[1] - self.target_position[1]))
                    
                    # Realizar movimiento
                    if self.is_valid_position(next_position):
                        self.move_to_position(next_position)
                else:
                        self.go_zone = False

            else:
                zero_positions = [(x, y) for y in range(len(model.colmena)) for x in range(len(model.colmena[0])) if model.colmena[y][x] == 0]
                if not zero_positions:
                    self.mapping = False
                    return
        
                zero_positions.sort(key=lambda pos: abs(pos[0] - self.pos[0]) + abs(pos[1] - self.pos[1]))
        
                target_position = zero_positions[0]
                possible_steps = self.get_filtered_possible_steps(self.pos)
                next_position = min(possible_steps, key=lambda pos: abs(pos[0] - target_position[0]) + abs(pos[1] - target_position[1]))
        
                if self.is_valid_position(next_position):
                    self.move_to_position(next_position)
                                
                self.map_colmena(self.get_possible_steps(self.pos))
                                
                            
        else:
            # Recoger basura o mover al basurero dependiendo de la capacidad actual
            possible_steps = self.get_filtered_possible_steps(self.pos)

            if not possible_steps:
                return

            if self.current_trash == self.trash_capacity:
                if self.trash_bin != self.pos:
                    next_position = min(possible_steps, key=lambda pos: abs(pos[0] - self.trash_bin[0]) + abs(pos[1] - self.trash_bin[1]))
                else:
                    self.current_trash = 0
                    next_position = random.choice(possible_steps)
            else:
                trash_positions = [(x, y) for y in range(len(model.colmena)) for x in range(len(model.colmena[0])) if model.colmena[y][x] == 1]

                if not trash_positions:
                    if self.trash_bin != self.pos:
                        next_position = min(possible_steps, key=lambda pos: abs(pos[0] - self.trash_bin[0]) + abs(pos[1] - self.trash_bin[1]))
                    else:
                        self.current_trash = 0
                        self.model.grid_data[self.pos[1]][self.pos[0]] = 103

                        self.model.grid.remove_agent(self)
                        self.model.schedule.remove(self)
                        
                        return 
                else:
                    # Ordenar las posiciones de basura según la distancia al robot.
                    trash_positions.sort(key=lambda pos: abs(pos[0] - self.pos[0]) + abs(pos[1] - self.pos[1]))
                    
                    target_position = trash_positions[0]
                    possible_steps = self.get_filtered_possible_steps(self.pos)
                    next_position = min(possible_steps, key=lambda pos: abs(pos[0] - target_position[0]) + abs(pos[1] - target_position[1]))
                    
                    if self.is_valid_position(next_position):
                        self.move_to_position(next_position)
                    
                        if self.original_cell_value > 0 and self.original_cell_value < 10:
                            trash = min(5 - self.current_trash, self.original_cell_value)
                            self.current_trash += trash
                            self.original_cell_value -= trash
                            
                            if self.original_cell_value == 0:
                                    self.model.colmena[self.pos[1]][self.pos[0]] = 50

            if self.is_valid_position(next_position):
                self.move_to_position(next_position)

    def map_colmena(self, possible_steps):
        for pos in possible_steps:
            cell_value = self.model.grid_data[pos[1]][pos[0]]
            
            # Chequeamos las condiciones dadas y modificamos colmena_matrix acorde
            if cell_value == 101:
                self.model.colmena[pos[1]][pos[0]] = 101
            elif cell_value == 0:
                self.model.colmena[pos[1]][pos[0]] = 50
            elif 1 <= cell_value <= 10:
                self.model.colmena[pos[1]][pos[0]] = 1
            elif cell_value == 102 or cell_value == 103:
                self.model.colmena[pos[1]][pos[0]] = 50


def read_input(filename):
    with open(filename, 'r') as file:
        n, m = map(int, file.readline().split())
        office_layout = [file.readline().split() for _ in range(n)]
        return n, m, office_layout

def get_grid(model):
    return model.grid_data.copy()

class Oficina(Model):
    def __init__(self, filename):
        self.filename = filename
        n, m, office_layout = read_input(filename)  
        self.n = n
        self.m = m
        self.grid = MultiGrid(m, n, torus = False)
        self.colmena = np.zeros((n, m))
        
        # Inicializa grid_data con valores basados en office_layout
        self.grid_data = np.zeros((n, m))

        self.trash_bin = None
        
        for i in range(n):
            for j in range(m):
                cell_value = office_layout[i][j]
                if cell_value == 'X':
                    self.grid_data[i][j] = 101
                elif cell_value == 'S':
                    self.grid_data[i][j] = 102
                elif cell_value == 'P':
                    self.grid_data[i][j] = 103
                    self.trash_bin = (j, i)
                else:
                    self.grid_data[i][j] = int(cell_value)
                
        self.schedule = RandomActivation(self)
        self.datacollector = DataCollector(model_reporters={"Grid": get_grid})
        
        for y in range(n):
            for x in range(m):
                if office_layout[y][x] == 'S':
                    for z in range(5):
                        if z == 0:
                            target_position = (0,0)
                        elif z == 1:
                            target_position = (0,n)
                        elif z == 2:
                            target_position = (m,n)
                        elif z == 3:
                            target_position = (m,0)
                        elif z == 4:
                            target_position = (m/2,n/2)
                        
                        robot = Robot(z+105, self, (x, y), self.trash_bin, target_position)
                        self.grid.place_agent(robot, (x, y))
                        self.schedule.add(robot)
        
    def step(self):
        self.datacollector.collect(self)
        self.schedule.step()
         

flag = True
model = Oficina("input3.txt")
MAX_ITERATIONS = (model.n * model.m)*4
for i in range(MAX_ITERATIONS):
    model.step()
    if len(model.schedule.agents) == 0 and flag == True :
        print(i)
        flag = False


count=0
count3=0
for y in range(len(model.colmena)):
        for x in range(len(model.colmena[0])):
            if model.colmena[y][x] == 1 :
                count+=1
            if model.colmena[y][x] == 0 :
                count3+=1

print(count)
print(count3)

all_grid = model.datacollector.get_model_vars_dataframe()

colors = ['white','red', 'black', 'blue', 'green', 'purple']
bounds = [0, 0.5, 100.5, 101.5, 102.5, 103.5, 200]  # Asumiendo que el ID del robot es < 200
norm = BoundaryNorm(bounds, len(colors))

cmap = ListedColormap(colors)

fig, axs = plt.subplots(figsize=(7,7))
axs.set_xticks([])
axs.set_yticks([])
patch = plt.imshow(all_grid.iloc[0][0], cmap=cmap, norm=norm)

def animate(i):
    if i < len(all_grid):
        numeric_grid = all_grid.iloc[i][0]

        # Limpia cualquier texto anterior
        for txt in axs.texts:
            txt.remove()

        robot_labels = {109: "A", 105: "B", 106: "C", 107: "D", 108: "E"}

        # Recorre la matriz y verifica cada valor
        for row_idx, row in enumerate(numeric_grid):
            for col_idx, value in enumerate(row):
                if value in robot_labels:
                    axs.text(col_idx, row_idx, robot_labels[value], ha='center', va='center', color='white')
                elif value not in [0, 101, 102, 103]:
                    # Muestra el valor en la celda roja como un entero
                    axs.text(col_idx, row_idx, str(int(value)), ha='center', va='center', color='white')

        patch.set_data(numeric_grid)

    else:
        pass
    
anim = animation.FuncAnimation(fig, animate, frames=MAX_ITERATIONS)
plt.show()