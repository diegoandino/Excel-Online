#TODO run the tests on the server
import  json
import socket
import threading
import sys

max_test_time = 20
number_of_test = 13

input = sys.argv # receive input 

if(len(input) >= 2):
    test_num =  input[1]
    address = input[2]
else:
    print(number_of_test)
    sys.exit()


#strings for tests
terminator = "\n"
messageterminator = "/n"
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
client_edit_cell = "{{requestType: \"editCell\", cellName: \"{0}\", contents: \"{1}\" }}".format(cell_name, cell_contents)
client_revert = "{{\"requestType\": \"revertCell\", \"cellName\": \"{0}\"}}".format(cell_name)
client_select_cell = "{{requestType: \"selectCell\", cellName: \"{0}\" }}".format(cell_name)

client_undo = "{\"requestType\": \"undo\"}"

client_handshake_filename = ("\"{0}\"" + terminator).format(filename1)
client_handshake_username = ("\"{0}\"" + terminator).format(user_name)

#server -> client
client_disconnected = "{{messageType: \"disconnected\", user: \"{0}\"}}".format(ID)

server_handshake_files = "{1}{0}{2}{0}{3}{0}{0}".format(terminator, filename1, filename2, filename3)

error_invalid_request = "{{ messageType: \"requestError\", cellName: {0},message: {1} }}".format(cell_name, error_message)
error_shutdown_server = "{{ messageType: \"serverError\", message: {0} }}".format(error_message)

server_cell_selected = "{{messageType: \"cellSelected\", + cellName: {0} selector: {1}, selectorName: {2} }}".format(cell_name, ID, user_name)
server_cell_changed = "{{ messageType: \"cellUpdated\", cellName: {0}, contents: {1} }}".format(cell_name, cell_contents)


#char array for cell name
letter = ['a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z']

# Get the id from the server
def TryParse(string):
    try:
        int(string)
        return True
    except ValueError:
       return  False

# Def Class TestClient
class TestClient:
    # Contructor
    def __init__(self, name):
        # create a class variable
        self.soc = None 
        self.clientname = name
        self.id = None

        
    # class's method connect_to_server
    def connect_to_server(self, address):
        self.soc = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        ip_ad = address.split(':')
        try:
            self.soc.settimeout(max_test_time)
            self.soc.connect( (ip_ad[0], int(ip_ad[1])) )
            self.soc.settimeout(None)
        except:
            print("Fail" + messageterminator)
            sys.exit()

        # Send the username to the server
        self.send_message(self.clientname)

        
    # class's method close_connection
    def close_connection(self):
        self.soc.shutdown()
        self.soc.close()
        
     # class's method send_message
    def send_message(self, msg):
        self.soc.send(bytes(msg, 'utf-8') + bytes(terminator, 'utf-8'))
        
     # class's method receiving JSON
    def receive(self):
        received= ""
                
        # Check if the \n is in the received
        while terminator not in received:
            try:
                self.soc.settimeout(max_test_time)
                received = self.soc.recv(1024).decode("utf-8")  # byte -> string
                self.soc.settimeout(None)
                if terminator not in received:
                    received += received
            except:
                print("Fail" + messageterminator)
                exit()    

  
        return received.rstrip()

    # class's method receiving a list of spreadsheet
    def receiveSpreadsheet(self):
        received= ""
        while "\n\n" not in received:
            try:
                self.soc.settimeout(max_test_time)
                received = self.soc.recv(1024).decode("utf-8") 
                self.soc.settimeout(None)
                if "\n\n" not in received:
                    received += received
            except:
                print("Fail" + messageterminator)
                exit()    

  
        return received

    # class's method receiving receiving spreadsheet selection and update when connecting to server
    def receiveSpreadsheetSelectionandUpdate(self):
        stringlist = []
        while True:
            try:
                self.soc.settimeout(max_test_time)
                received = self.soc.recv(1024).decode("utf-8") 
                self.soc.settimeout(None)
                
                # Get rid of \n from the received
                if TryParse(received.rstrip()):
                    id = int(received.rstrip())
                    break;
                else:
                    stringlist.append(received.rstrip())
            except:
                print("Fail" + messageterminator)
                exit()    

  
        return received


#The end of the class def


#methods defs for JSON
def DisconnectedString(ID):
    return '{{"messageType": "disconnected", user: "{0}"}}'.format(ID)
def SelectCell(cell_name):
     return '{{"requestType": "selectCell", "cellName": "{0}" }}'.format(cell_name)
def EditCell(cell_name, cell_contents):
    return '{{"requestType": "editCell", "cellName": "{0}", "contents": "{1}" }}'.format(cell_name, cell_contents)
def RevertCell(cell_name):
    return '{"requestType": "revertCell", "cellName": "{0}"}}'.format(cell_name)


#server_handshake_files = "{1}{0}{2}{0}{3}{0}{0}".format(terminator, file1, file2, file3)


#error_invalid_request = "{{ messageType: \"requestError\", cellName: {0},message: {1} }}".format(cell_name, error_message)
#error_shutdown_server = "{{ messageType: \"serverError\", message: {0} }}".format(error_message)

#server_cell_selected = "{{messageType: \"cellSelected\", + cellName: {0} selector: {1}, selectorName: {2} }}".format(cell_name, ID, user_name)
#server_cell_changed = "{{ messageType: \"cellUpdated\", cellName: {0}, contents: {1} }}".format(cell_name, cell_contents)



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
    elif(num == '4'):
        test_4(address)
    elif(num=='5'):
        test_5(address)
    elif(num =='6'):
        test_6(address)
    elif(num=='7'):
        test_7(address)
    elif(num=='8'):
        test_8(address)
    elif(num=='9'):
        test_9(address)
    elif(num=='10'):
        test_stress(address)
    elif(num=='11'):
        test_stress2(address)
    elif(num=='12'):
        test_stress3(address)

def set_connection(ip, port):
    try:
        sckt.settimeout(max_test_time)
        sckt.connect( (ip, port) )
        sckt.settimeout(None)

    except socket.timeout:
        print("Failed" + messageterminator)
        sys.exit()

    except socket.error:
        print("Failed"  + messageterminator)
        sys.exit()

def receive_test():
    try:
        sckt.settimeout(max_test_time)
        received = sckt.recv(1024)
        sckt.settimeout(None)

    except socket.timeout:
        print("Fail" + messageterminator)
        sys.exit()

    except socket.error:
        print("Fail" + messageterminator)
        sys.exit()

    return received

# Tests
def test_1(address):
    
    print(max_test_time)
    print("Testing receive")

    ip_ad = address.split(':')
    set_connection(ip_ad[0], int(ip_ad[1]) )
    send_message("client" + terminator)
    receive_test()
    print("Pass" + messageterminator)
    return

def test_2(address):
    print(max_test_time)
    print("Testing edit for one client")
    client = TestClient("client");
    client.connect_to_server(address)
    if client.receiveSpreadsheet() != "\n\n":
        print("Fail" + messageterminator)
        return
    client.send_message("file")
    string =client.receiveSpreadsheetSelectionandUpdate()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "1"))
    try:
        x= json.loads(client.receive())
        if  "cellUpdated" not in x["messageType"] or  "A1" not in x["cellName"] or "1" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    print("Pass" + messageterminator)
    return
    


def test_3(address):
    print(max_test_time)
    print("Testing edits for one client")
    client = TestClient("client");
    client.connect_to_server(address)
    client.receiveSpreadsheet()
    client.send_message("file")
    client.receiveSpreadsheetSelectionandUpdate()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", 1))
    try:
        x= json.loads(client.receive())
        if  "cellUpdated" not in x["messageType"] or "A1" not in x["cellName"] or "1" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A4"))
    client.send_message(EditCell("A4", 5))
    try:
        x= json.loads(client.receive())
        if  "cellUpdated" not in x["messageType"] or "A4" not in x["cellName"] or "5" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A5"))
    client.send_message(EditCell("A5", "=A1 + A4"))
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"] or "A5" not in x["cellName"] or "=A1 + A4" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A5"))
    client.send_message(EditCell("A5", "Hello World"))
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"] or "A5" not in x["cellName"] or "Hello World" not in x["contents"]:
            print("Fail" + messageterminator)
        return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A7"))
    client.send_message(EditCell("A7", "3"))
    try:
        x= json.load(client.receive())
        if "cellUpdated" not in x["messageType"] or "A7" not in x["cellName"] or "3" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    print("Pass" + messageterminator)
    r
    
def test_4(address):
    print(max_test_time)
    print("testing Undo")
    client = TestClient("client");
    client.connect_to_server(address)
    client.receiveSpreadsheet()
    client.send_message("file")
    client.receiveSpreadsheetSelectionandUpdate();
    client.send_message(SelectCell("A1"))
    client.send_message(client_undo)
    try:
        x= json.loads(client.receive())
        if "requestError" not in x["messageType"]  or "A1"  not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(EditCell("A1", "3"))
    client.receive()
    client.send_message(client_undo)
    try:
        x= json.loads(client.receive())
        if  "cellUpdated" not in x["messageType"] or "A1" not in x["cellName"] or  not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "=3 + 4"))
    client.receive()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", 3))
    client.receive()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "H"))
    client.receive()
    client.send_message(client_undo)
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"] or "A1"not in x["cellName"] or "H" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(client_undo)
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "3" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(client_undo)
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "12" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    print("Pass" + messageterminator)

def test_5(address):
    print(max_test_time)
    print("Testing Redo")
    client = TestClient("client");
    client.connect_to_server(address)
    client.receiveSpreadsheet()
    client.send_message("file")
    client.receiveSpreadsheetSelectionandUpdate()
    client.send_message(SelectCell("A1"))
    client.send_message(RevertCell("A1"))
    try:
        x= json.loads(client.receive())
        if "requestError" not in x["messageType"]  or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "=3 + 5"))
    client.recieve()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "What"))
    client.recieve()
    client.send_message(SelectCell("A1"))
    client.send_message(EditCell("A1", "Hello"))
    client.receive()
    client.send_message(RevertCell("A1"))
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "What" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(RevertCell("A1"))
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "15" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
        client.send_message(RevertCell("A1"))
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
    print("Pass" + messageterminator)

def test_6(address):
    print(max_test_time)
    print("Testing Redo and Undo for multiple clients")
    client = TestClient("client")
    client.connect_to_server(address)
    client.receiveSpreadsheet()
    client.send_message("file")
    client.receiveSpreadsheetSelectionandUpdate()
    client2 = TestClient("client2");
    client2.connect_to_server(address)
    
    if client2.receiveSpreadsheet() != "file/n/n":
        print("Fail" + messageterminator)
        return
    client.send_message("file")
    client2.receiveSpreadsheetSelectionandUpdate()
    client.send_message(SelectCell("A1"))
    try:
        x= json.load(client2.receive())
        if "cellSelected" not in x["messageType"]  or "A1" not in x["cellName"] or str(client.id) not in x["selector"] or str(client.clientname) not in x["selectorName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(RevertCell("A1"))
    
    try:
        x= json.load(client.receive())
        
        if  "requestError" not in x["messageType"] or  "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    try:
        x= json.load(client2.receive())
        if "requestError" not in x["messageType"] or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client2.send_message(SelectCell("A1"))
    try:
        x= json.loads(client.receive())
        if "cellSelected" not in x["messageType"]  or "A1" not in x["cellName"] or str(client2.id) not in x["selector"] or str(client2.clientname) not in x["selectorName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client2.send_message(EditCell("A1", "3"))
    try:
        x= json.loads(client2.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "3" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "3" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
    client2.send_message(client_undo)
    try:
        x= json.loads(client2.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
        try:
            x= json.loads(client.receive())
            if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
                print("Fail" + messageterminator)
                return
        except:
            print("Fail" + messageterminator)
            return
    client2.send_message(SelectCell("A1"))
    client.receive()
    client2.send_message(EditCell("A1", "Hello"))
    client2.receive()
    client.receive()
    client.send_message(SelectCell("A2"))
    client2.receive()
    client.send_message(EditCell("A2", "See"))
    client2.receive()
    client.receive()
    client.send_message(SelectCell("A3"))
    client2.receive()
    client.send_message(EditCell("A3", "World"))
    client2.receive()
    client.receive()
    client.send_message(RevertCell("A1"))
    try:
        x= json.loads(client2.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client.send_message(client_undo)
    try:
        x= json.loads(client2.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or "Hello" not in x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    
    try:
        x= json.loads(client.receive())
        if "cellUpdated" not in x["messageType"]  or "A1" not in x["cellName"] or not x["contents"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return   
    print("Pass" + messageterminator)

    
def test_7(address):
    print(max_test_time)
    print("testing closing connection")
    client = TestClient("client")
    client2 = TestClient("client")
    client.connect_to_server(address)
    client2.connect_to_server(address)
    client.receiveSpreadsheet()
    client2.receiveSpreadsheet()
    client.send_message("file")
    client2.send_message("file")
    client.close_connection()
    try:
        x = json.loads(client2.receive())
        if "disconnected" not in x["messageType"] or str(client.id) not in x["user"]:
            print("Fail" + messageterminator)
        return
    except:
        print("Fail" + messageterminator)
        return
    print("Pass" + messageterminator)
    return


def test_8(address):
    print(max_test_time)
    print("testing circular dependencies")
    client = TestClient("client")
    client2 = TestClient("client")
    client.connect_to_server(address)
    client2.connect_to_server(address)
    client.receiveSpreadsheet()
    client2.receiveSpreadsheet()
    client.send_message("file")
    client2.send_message("file")
    client2.send_message(SelectCell("A1"))
    client.receive()
    client2.send_message(EditCell("A1", "=A1"))
    try:
        x= json.loads(client.receive())
        if "requestError" not in x["messageType"] or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    try:
        x= json.loads(client2.receive())
        if "requestError" not in x["messageType"] or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client2.send_message(SelectCell("A1"))
    client.receive()
    client2.send_message(EditCell("A1", "=A1 + A1"))
    try:
        x= json.loads(client.receive())
        if "requestError" not in x["messageType"] or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    try:
        x= json.loads(client2.receive())
        if "requestError" not in x["messageType"] or "A1" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    client2.send_message(SelectCell("A1"))
    client2.send_message(EditCell("A1", "=A3 + A2"))
    client1.send_message(SelectCell("A3"))
    client2.send_message(EditCell("A3", "=A1 + A8"))
    try:
        x= json.loads(client.receive())
        if "requestError" not in x["messageType"] or "A3" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    try:
        x= json.loads(client2.receive())
        if "requestError" not in x["messageType"] or "A3" not in x["cellName"]:
            print("Fail" + messageterminator)
            return
    except:
        print("Fail" + messageterminator)
        return
    print("Pass" + messageterminator)
    return

def test_9(address):
    print(max_test_time)
    print("testing file list")
    client = TestClient("client")
    endFile = r"\n"
    
    for i in range(0,10):
        client.connect_to_server(address)
        if(client.receive() != file):
            print("Fail" + messageterminator)
            return
        s = "file" + i
        client.send_message(s)
        endFile = s + r"\n" + endFile
        client.close_connection()
    print("Pass" + messageterminator)
    return


def test_stress(address):
    print(max_test_time)
    print("stress test client edit all cells")
    client = TestClient("client")
    client.connect_to_server(address)
    client.receive()
    client.send_message("file")
    string =client.receiveSpreadsheetSelectionandUpdate()
    for i in range(0, 100):
        for a in letter:
            client.send_message(SelectCell(a+i))
            client.send_message(EditCell(a+i, i))
            client.receive()
            client.send_message(SelectCell(a+i))
            client.send_message(EditCell(a+i, "Hello"))
            client.receive()
            client.send_message(SelectCell(a+i))
            client.send_message(EditCell(a+i, "= 3 + 10"))
            client.receive()
    
    print("Pass" + messageterminator)
    return
        
def test_stress2(address):
    print(max_test_time)
    print("stress test 2 multiple clients")
    client = TestClient("client")
    client.connect_to_server(address)
    client.receive()
    client.send_message("file")
    client.receiveSpreadsheetSelectionandUpdate()
    client2 = TestClient("client2")
    client2.connect_to_server(address)
    client2.receiveSpreadsheet()
    client2.send_message("file")
    client2.receiveSpreadsheetSelectionandUpdate()
    client3 = TestClient("client3" + terminator);
    client3.connect_to_server(address)
    client3.receiveSpreadsheet()
    client3.send_message("file")
    client3.receiveSpreadsheetSelectionandUpdate()

    for i in range(0, 100):
        for a in letter:
            client.send_message(SelectCell(a+i))
            client2.receive()
            client3.receive()
            client.send_message(EditCell(a+i, "1"))
            client.receive()
            client2.receive()
            client3.receive()
            client2.send_message(SelectCell(a+i))
            client.receive()
            client3.receive()
            client2.send_message(EditCell(a+i, "2"))
            client.receive()
            client2.receive()
            client3.receive()
            client3.send_message(SelectCell(a+i))
            client.receive()
            client2.receive()
            client3.send_message(EditCell(a+i, "3"))
            client.receive()
            client2.receive()
            client3.receive()
    
    print("Pass" + messageterminator)
    return

def test_sttess3():
    for i in range(0, 25):
        client.send_message(SelectCell("A1"))
        client.send_message(EditCell("A1", "=3"))
        client.receive()
        client.send_message(SelectCell("A1"))
        client.send_message(EditCell("A1", "= 20 +3"))
        client.receive()
        client.send_message(SelectCell("A1"))
        client.send_message(EditCell("A1", "Hello"))
        client.receive()
        client.send_message(SelectCell("A1"))
        client.send_message(EditCell("A1", "32"))
        client.receive()
    for i in range(0, 25):
        client.send_message(client_undo)
        client.receive()
    print("Pass" + messageterminator)
    return
    

#from https://www.geeksforgeeks.org/socket-programming-python/

sckt = socket.socket(socket.AF_INET, socket.SOCK_STREAM)



run_input(test_num, address)

close_connection()
sys.exit()
