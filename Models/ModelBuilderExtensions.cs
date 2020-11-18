using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 3,
                    Name = "Super Guili",
                    Email = "superguili@gmail.com",
                    Department = Dept.IT
                },
                new Employee
                {
                    Id = 4,
                    Name = "Test number 4444",
                    Email = "test444@4444.com",
                    Department = Dept.Payroll
                }
                );
        }
    }
}
