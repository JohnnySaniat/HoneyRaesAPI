using HoneyRaesAPI.Models;

List<Customer> customers = new List<Customer>()
{
    new Customer()

{
    Id = 1,
    Name = "Lisette Sanby",
    Address = "42 Wallaby Way",
},
    new Customer()

{
    Id = 2,
    Name = "Jarn Sanby",
    Address = "2848 Roslyn St.",
},
    new Customer()

{
    Id = 3,
    Name = "Blarn Barby",
    Address = "1705 Golf St.",
}
};

List<Employee> employees = new List<Employee>()
{
    new Employee()

{
    Id = 1,
    Name = "Marg Fin",
    Specialty = "Karate",
},
    new Employee()

{
    Id = 2,
    Name = "Kip Doe",
    Specialty = "Barkin'",

},
    new Employee()
    {
        Id = 3,
        Name = "Oingo Boingo",
        Specialty = "Boinkin"
    }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>()
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Help me please",
        Emergency = true,
        DateCompleted = new DateTime(2023, 5, 15),
    },

    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = null,
        Description = "What",
        Emergency = true,
        DateCompleted = null,
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "SO COLD",
        Emergency = false,
        DateCompleted = new DateTime(2023, 6, 20),
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 1,
        EmployeeId = null,
        Description = "Is anyone there?",
        Emergency = false,
        DateCompleted = new DateTime(2024, 1, 15),
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Bozo!",
        Emergency = true,
        DateCompleted = null,
    },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);

});

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});
app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(e => e.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    List<ServiceTicket> customerServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    customer.ServiceTickets = customerServiceTickets;

    return Results.Ok(customer);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicketToDelete = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicketToDelete == null)
    {
        return Results.NotFound();
    }

    serviceTickets.Remove(serviceTicketToDelete);

    return Results.NoContent();
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket updatedServiceTicket) =>
{
    ServiceTicket existingServiceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (existingServiceTicket == null)
    {
        return Results.NotFound();
    }

    existingServiceTicket.EmployeeId = updatedServiceTicket.EmployeeId;

    return Results.Ok(existingServiceTicket);
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

app.MapGet("/emergencies", () =>
{
    var emergencyTickets = serviceTickets.Where(st => st.Emergency && st.DateCompleted == null).ToList();

    foreach (var serviceTicket in emergencyTickets)
    {
        serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    }

    return Results.Ok(emergencyTickets);


});

app.MapGet("/unassigned", () =>
{
    var unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();

    foreach (var serviceTicket in unassignedTickets)
    {
        serviceTicket.Customer = customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    }

    return Results.Ok(unassignedTickets);
});

app.MapGet("/inactivecustomers", () =>
{
    var inactiveCustomers = customers
        .Where(c =>
            !serviceTickets.Any(st =>
                st.CustomerId == c.Id &&
                st.DateCompleted.HasValue && st.DateCompleted.Value > DateTime.Now.AddYears(-1)
            )
        )
        .ToList();

    return Results.Ok(inactiveCustomers);
});

app.MapGet("/availableemployees", () =>
{
    var assignedEmployees = serviceTickets.Where(st => st.EmployeeId.HasValue).Select(st => st.EmployeeId.Value).ToList();
    var availableEmployees = employees.Where(e => !assignedEmployees.Contains(e.Id)).ToList();
    return Results.Ok(availableEmployees);
});

app.MapGet("/employees/{id}/customers", (int id) =>
{
    var employeeCustomers = serviceTickets
        .Where(st => st.EmployeeId == id)
        .Select(st => customers.FirstOrDefault(c => c.Id == st.CustomerId))
        .Distinct()
        .ToList();

    return Results.Ok(employeeCustomers);
});

app.MapGet("/employeeofthemonth", () =>
{
    var lastMonth = DateTime.Now.AddMonths(-1);
    var employeeOfTheMonth = employees
        .OrderByDescending(e => serviceTickets.Count(st => st.EmployeeId == e.Id && st.DateCompleted.HasValue && st.DateCompleted.Value.Month == lastMonth.Month))
        .FirstOrDefault();

    return Results.Ok(employeeOfTheMonth);
});

app.MapGet("/pastticketreview", () =>
{
    var completedTickets = serviceTickets
        .Where(st => st.DateCompleted.HasValue)
        .OrderBy(st => st.DateCompleted)
        .ToList();

    foreach (var ticket in completedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(completedTickets);
});

app.MapGet("/prioritizedtickets", () =>
{
    var prioritizedTickets = serviceTickets
        .Where(st => !st.DateCompleted.HasValue)
        .OrderByDescending(st => st.Emergency)
        .ThenBy(st => st.EmployeeId.HasValue) 
        .ToList();

    foreach (var ticket in prioritizedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(prioritizedTickets);
});

app.Run();

