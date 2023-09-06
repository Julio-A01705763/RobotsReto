# TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
# Python server to interact with Unity via POST
# Sergio Ruiz-Loza, Ph.D. March 2021

from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json

class Server(BaseHTTPRequestHandler):
    current_simulation = None

    def _send_response(self, content, status=200):
      self.send_response(status)
      self.send_header('Content-type', 'application/json')
      self.end_headers()
      self.wfile.write(bytes(content, "utf8"))
      
    def do_GET(self):
      if self.path == "/get-simulation":
        if Server.current_simulation:
          self._send_response(json.dumps(Server.current_simulation))
        else:
          self._send_response('{"message": "No simulation available!"}', 404)
      else:
        self._send_response('{"message": "Endpoint not found!"}', 404)

    def do_POST(self):
      if self.path == "/update-simulation":
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length)
        Server.current_simulation = json.loads(post_data)

        self._send_response('{"message": "Simulation updated!"}')
      else:
        self._send_response('{"message": "Endpoint not found!"}', 404)

      

def run(server_class=HTTPServer, handler_class=Server, port=8585):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info("Starting httpd...\n") # HTTPD is HTTP Daemon!
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:   # CTRL+C stops the server
        pass
    httpd.server_close()
    logging.info("Stopping httpd...\n")

if __name__ == '__main__':
    from sys import argv
    
    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()
