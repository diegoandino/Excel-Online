Entry Date: 4/27/2021
-----------------------------------------------------
Authors: Tarik Vu, Diego Andino, Max Mcloughlin, James Gibb, Brady Tan, Vicente Almeida 


Design Decisions: 
	- As per instructions of Jakkpot, we kept the core functionality between the server and the client(s) seperate where, 
	The client's main duty were to update their gui's appropriately and to send appropriate requests to the server for actions 
	such as editing, reverting, and undoing changes. 

	- Using alot of source code from Diego and Tarik's previous spreadsheet assignment, many things were adopted and adjusted in order
	to facilitate the communication between a server and client.  Code such as busy loops, processing of messages, etc. were used here.

	- Server-sided code was implemented in c++ while client-side code remained in c# for the sake of simplicity.

Challenges of PS6:
	-Teamwork: As this was many of our first times working in such a large group, we found the delegations of tasks as well as having specific short-term 
	milestones to be difficult to clarify.
	
	-Implementation: 
			* Translating c# to c++
			* c++ in general
			* Handshake and communication between the client and server.

	-Teaching Assistance:
			* Due to the nature of this assignment it was difficult to get comprehensive help.  This can be derived from the fact that with so many different
			  groups of people having different implementations, TA help seemed to be very generalized and did not always solve specific issues.

Final notes: 
	As our team steadily worked on this assignment over the course of 3 weeks, we practiced alot of pair programming in groups of two, split between the client and the server.
	Server-sided code was mainly spearheaded by one group member.  As for client-side design the main changes made were to add compatibility for server connections.
	