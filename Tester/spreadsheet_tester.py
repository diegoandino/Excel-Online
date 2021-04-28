#import  json
import socket
import threading
import sys


max_test_time = 10
number_of_test = 7;

input = sys.argv

if(len(input) >= 2):
    test_num =  input[1]
    address = input[2]
else:
    print(number_of_test)
    quit()




#strings for tests
terminator = r"\n"
messageterminator = "/n"
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


class TestClient:
    def __init__(self, name):
        self.soc = None
        self.clientname = name

    def connect_to_server(self, address):
        self.soc = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        ip_ad = address.split(':')
        try:
            self.soc.settimeout(max_test_time)
            self.soc.connect( (ip_ad[0], int(ip_ad[1])) )
            self.soc.settimeout(None)
        except:
            print("Fail" + messageterminator)
            exit()
    def close_connection():
        sckt.close


    def send_message(self, msg):
        self.soc.send(bytes(msg, 'utf-8') + bytes(terminator, 'utf-8'))
        #print(msg + " sent.")

    def receive(self):
        stringlist = []
        while True:
            try:
                self.soc.settimeout(max_test_time)
                received = self.soc.recv(1024)
                self.soc.settimeout(None)
                if len(received) == 0:
                    break
                else:
                    stringlist.append(received)
            except:
                print("Fail" + messageterminator)
                exit()
           #        received= ""
            #       while "\n" not in received:

  
        return received





#methods defs
def DisconnectedString(ID):
    return "{messageType: \"disconnected\", user: \"" + ID + "\"}"
def SelectCell(cell_name):
     return "{requestType: \"selectCell\", cellName: " + cell_name + " }"
def EditCell(cell_name, cell_contents):
    return "{requestType: \"editCell\", cellName: " + cell_name + ", contents: " + cell_contents + " }"

def SendClientName(clientname):
    return clientname + terminator

def SendFileName(file):
    return file+ terminator





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
    if(num == '1'):
        test_1(address)
    elif(num == '2'):
        test_2(address)
    elif(num == '3'):
        test_3(address)
        

def set_connection(ip, port):
    try:
        sckt.settimeout(max_test_time)
        sckt.connect( (ip, port) )
        sckt.settimeout(None)

    except socket.timeout:
        print("Failed" + messageterminator)
        exit()

    except socket.error:
        print("Failed"  + messageterminator)
        exit()

def receive_test():
    try:
        sckt.settimeout(max_test_time)
        received = sckt.recv(1024)
        sckt.settimeout(None)

    except socket.timeout:
        print("Fail" + messageterminator)
        exit()

    except socket.error:
        print("Fail" + messageterminator)
        exit()

    return received


def test_1(address):
    
    print(max_test_time)
    print("test receive #1")

    ip_ad = address.split(':')
    set_connection(ip_ad[0], int(ip_ad[1]) )
    send_message("client" + terminator)
    receive_test()
    print("Pass" + messageterminator)

def test_2(address):
    print(max_test_time)
    print("test edit for one client")
    client = TestClient("client" + terminator);
    client.connect_to_server(address)
    client.receive()
    client.send_message(client.clientname)
    client.receive()
    client.send_message(SendFileName("file"))
    string =client.receive()
    client.send_message(SelectCell("A1"))
    client.receive()
    client.send_message(EditCell("A1", "1"))
    client.receive()
    print("Pass" + messageterminator)
    


def test_3(address):
    print(max_test_time)
    print("testing a lot of edits for one client")
    client = TestClient("client" +terminator);
    client.connect_to_server(address)
    client.send_message(client.clientname)
    client.receive()
    client.send_message(SendFileName("file"))
    string =client.receive()
    if string.count() != 0:
        print("Fail" + messageterminator)
    client.send_message(SelectCell("A1"))
    client.receive()
    client.send_message(EditCell("A1", "1"))
    client.receive()
    client.send_message(SelectCell("A4"))
    client.receive()
    client.send_message(EditCell("A4", "5"))
    client.receive()
    client.send_message(SelectCell("A5"))
    client.receive()
    client.send_message(EditCell("A5", "=A1 + A4"))
    client.receive()
    client.send_message(SelectCell("A5"))
    client.receive()
    client.send_message(EditCell("A5", "Hello World"))
    client.receive()
    client.send_message(SelectCell("A7"))
    client.receive()
    client.send_message(EditCell("A7", "=3"))
    client.receive()
    print("Pass" + messageterminator)
def test_4(address):
    print(max_test_time)
    print("testing Undo")
    client = TestClient("client" + terminator);
    client.connect_to_server(address)
    client.send_message(client.clientname)
    client.receive()
    client.send_message(SendFileName("file"))
    client.send_message(SelectCell("A1"))
    client.receive()
    client.send_message(client_undo)
    client.receive()
    client.send_message(EditCell("A1", "=3"))
    client.receive()
    client.send_message(client_undo)
    client.receive()
    print("Pass" + messageterminator)


def test_5(address):
    print(max_test_time)
    print("test editing all cells")
    client = TestClient("client" + terminator);
    client.connect_to_server(address)
    client.send_message(client.clientname)
    client.receive()
    client.send_message(SendFileName("file"))
    string =client.receive()
    letter = ['a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z']
    for i in range(0, 100):
        for a in letter:
            client.send_message(SelectCell(a+i))
            client.receive()
            client.send_message(EditCell(a+i, "1"))
            client.receive()
    
    print("Pass" + messageterminator)
        
def test_6(address):
    print(max_test_time)
    print("test multi client editing all cells")
    client = TestClient("client" + terminator);
    client2 = TestClient("client" + terminator);
    client3 = TestClient("client" + terminator);
    client.connect_to_server(address)
    client.send_message(client.clientname)
    client.receive()
    client.send_message(SendFileName("file"))
    string =client.receive()
    client = TestClient("client" + terminator);
    client.connect_to_server(address)
    client.send_message(client.clientname)
    client.receive()
    client2 = TestClient("client2" + terminator);
    client2.connect_to_server(address)
    client2.send_message(client.clientname)
    client2.receive()
    client2.send_message(SendFileName("file"))
    client2.receive()
    client3 = TestClient("client3" + terminator);
    client3.connect_to_server(address)
    client3.send_message(client.clientname)
    client3.receive()
    client3.send_message(SendFileName("file"))
    letter = ['a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z']

    for i in range(0, 100):
        for a in letter:
            client.send_message(SelectCell(a+i))
            client.receive()
            client.send_message(EditCell(a+i, "1"))
            client.receive()
            client2.send_message(SelectCell(a+i))
            client2.receive()
            client2.send_message(EditCell(a+i, "2"))
            client2.receive()
            client2.send_message(SelectCell(a+i))
            client2.receive()
            client2.send_message(EditCell(a+i, "3"))
            client2.receive()
    
    print("Pass" + messageterminator)

#from https://www.geeksforgeeks.org/socket-programming-python/

sckt = socket.socket(socket.AF_INET, socket.SOCK_STREAM)



run_input(test_num, address)

close_connection()
exit()
