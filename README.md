# Service Ticket Management System

This is a simple service ticket management system implemented in C# using HoneyRaesAPI.Models.

## Overview

The system includes three main entities: Customers, Employees, and Service Tickets.

### Customers

A list of customers with their details, including name and address.

### Employees

A list of employees with their details, including name and specialty.

### Service Tickets

A list of service tickets raised by customers. Each service ticket has a unique identifier, a reference to the customer, a reference to the employee assigned (if any), a description of the issue, an indication of emergency status, and the date when the service was completed.

## Code Structure

The code uses ASP.NET Core to create a simple web application. The application exposes various endpoints to interact with the data:

- `/servicetickets`: Get a list of all service tickets.
- `/servicetickets/{id}`: Get details of a specific service ticket by ID.
- `/employees`: Get a list of all employees.
- `/employees/{id}`: Get details of a specific employee by ID.
- `/customers`: Get a list of all customers.
- `/customers/{id}`: Get details of a specific customer by ID.
- `/servicetickets`: Create a new service ticket.
- `/servicetickets/{id}`: Delete a service ticket by ID.
- `/servicetickets/{id}`: Update a service ticket by ID.
- `/servicetickets/{id}/complete`: Mark a service ticket as completed.

## Postman

