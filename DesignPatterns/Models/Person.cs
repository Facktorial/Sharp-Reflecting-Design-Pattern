using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    public class Person
    {
        public Person()
        {
            Name = "New object";
            Age = -1;
        }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public int SALARY_CONSTANT = 1600;

        public string Name { get; set; }
        public int Age { get; set; }

        public int CalculateSalary() => SALARY_CONSTANT * Age;

        public int Calculate(int a, int b) => a * b * Age;

        public void Hello() => Console.WriteLine("Hello");

        public override string ToString()
        {
            return $"Name: {Name}, Age: {Age}, Salary: {CalculateSalary()}";
        }
    }
}
