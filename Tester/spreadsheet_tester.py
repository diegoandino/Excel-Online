import socket
import sys

terminator = "\n"

test_num = 1
max_test_time = 10

#strings for tests
cell_name = "a1"
cell_contents = "yo"
ID = "42"
user_name = "heyo"
error_message = "error"
filename1 = "file1"
filename2 = "file2"
filename3 = "file3"

#got these from the doc but some seem to be missing ','

#client -> server 

client_handshake_username = ("\"{0}\"" + terminator).format(user_name)
client_handshake_filename = ("\"{0}\"" + terminator).format(filename1)

client_edit_cell = "{{requestType: \"editCell\", cellName: \"{0}\", contents: \"{1}\" }}".format(cell_name, cell_contents)
client_revert = "{{\"requestType\": \"revertCell\", \"cellName\": \"{0}\"}}".format(cell_name)
client_select_cell = "{{requestType: \"selectCell\", cellName: \"{0}\" }}".format(cell_name)

client_undo = "{\"requestType\": \"undo\"}"

#server -> client
client_disconnected = "{{messageType: \"disconnected\", user: \"{0}\"}}".format(ID)
server_handshake_files = "{1}{0}{2}{0}{3}{0}{0}".format(terminator, filename1, filename2, filename3)

error_invalid_request = "{{ messageType: \"requestError\", cellName: {0},message: {1} }}".format(cell_name, error_message)
error_shutdown_server = "{{ messageType: \"serverError\", message: {0} }}".format(error_message)

server_cell_selected = "{{messageType: \"cellSelected\", + cellName: {0} selector: {1}, selectorName: {2} }}".format(cell_name, ID, user_name)
server_cell_changed = "{{ messageType: \"cellUpdated\", cellName: {0}, contents: {1} }}".format(cell_name, cell_contents)

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
    elif(num == 2):
        print("not implemented yet")

def set_connection(ip, port):
    try:
        sckt.settimeout(max_test_time)
        sckt.connect( (ip, port) )
        sckt.settimeout(None)

    except socket.timeout:
        sys.exit("socket timeout")

    except socket.error:
        sys.exit("socket error")

def receive_test():
    try:
        sckt.settimeout(max_test_time)
        received = sckt.recv(1024)
        sckt.settimeout(None)

    except socket.timeout:
        sys.exit("failed connection timed out")

    except socket.error:
        sys.exit("connection failed")

    return received

def set_up_test(test_name, message, address):
    print(max_test_time)
    print(test_name)

    ip_ad = address.split(':')
    set_connection(ip_ad[0], int(ip_ad[1]))

    send_message(message)

def test_1(address):
    set_up_test("test select cell #1", client_select_cell, address)
    receive_test()

    print("passed")

def test_send_filename():
    print("not implemented yet")

#from https://www.geeksforgeeks.org/socket-programming-python/

sckt = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

if(len(sys.argv) < 1):
    print("unh something went wrong, too *few* arguments?")
elif len(sys.argv) == 1:
    print(test_num)
elif len(sys.argv) == 3:

    try:
        arg1 = int(sys.argv[1])
    except ValueError:
        sys.exit("wrong first parameter inputed")

    run_input(arg1, sys.argv[2])

else:
    print("unh something went wrong, too *many* arguments?")

close_connection()
