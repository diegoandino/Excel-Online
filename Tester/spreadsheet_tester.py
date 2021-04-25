#import  json
import socket
import threading

terminator = "\n"

test_num = 1
max_test_time = 10

#strings for tests
cell_name = "a1"
cell_contents = "yo"
ID = "42"
name = "heyo"
error_message = "error"
filename1 = "file1"
filename2 = "file2"
filename3 = "file3"

#got these from the doc but some seem to be missing ','

#client -> server 
client_disconnected = "{messageType: \"disconnected\", user: \"" + ID + "\"}"

client_select_cell = "{requestType: \"selectCell\", cellName: " + cell_name + " }"
client_request_edit = "{requestType: \"editCell\", cellName: " + cell_name + ", contents: " + cell_contents + " }"
client_undo = "{\"requestType\": \"undo\"}"
client_revert = "{\"requestType\": \"revertCell\", \"cellName\": \"A1\"}"

client_handshake_username = "\"" + name + "\"" + terminator
client_handshake_filename = "\"" + filename1 + "\"" + terminator

#server -> client
server_handshake_files = "\"" + filename1 + "\"" + terminator + "\"" + \
    filename2 + "\"" + terminator + "\"" + \
    filename1 + "\"" + terminator + terminator

error_invalid_request = "{ messageType: \"requestError\", cellName: " + cell_name + ",message: " + error_message + " }"
error_shutdown_server = "{ messageType: \"serverError\", message: " + error_message + " }"

server_cell_selected = "{messageType: \"cellSelected\", + cellName: " + cell_name + " selector: " + ID + ", selectorName: " + name + " }"
server_cell_changed = "{ messageType: \"cellUpdated\", cellName: " + cell_name + ", contents: " + cell_contents + "}"

#methods defs
def send_message(msg):
    sckt.send(bytes(msg, 'utf-8') + bytes(terminator, 'utf-8'))
    #print(msg + " sent.")

def close_connection():
    sckt.close

def receive_msg():
    while True:
        print()
        print(b"received: " + sckt.recv(1024))
        print("input test to run ")

def run_input(num, address):
    if(num > test_num):
        print("no such test")
    elif(num == 1):
        test_1(address)
        #print("not implemented yet")
    elif(num == 2):
        print("not implemented yet")

def set_connection(ip, port):
    try:
        sckt.settimeout(max_test_time)
        sckt.connect( (ip, port) )
        sckt.settimeout(None)

    except socket.timeout:
        print("test failed")
        exit()

    except socket.error:
        print("test failed")
        exit()

def receive_test():
    try:
        sckt.settimeout(max_test_time)
        received = sckt.recv(1024)
        sckt.settimeout(None)

    except socket.timeout:
        print("failed connection timed out")
        exit()

    except socket.error:
        print("connection failed")
        exit()

    return received


def test_1(address):

    print(max_test_time)
    print("test select cell #1")

    ip_ad = address.split(':')
    set_connection(ip_ad[0], int(ip_ad[1]) )
    send_message(client_select_cell)

    receive_test()

    print("passed")

#from https://www.geeksforgeeks.org/socket-programming-python/


sckt = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

user_input = input("input test to run ")

if user_input == "":
    print(test_num)
else:
    intput = int(user_input)
    run_input(intput, "localhost:1100")

close_connection()
exit()
