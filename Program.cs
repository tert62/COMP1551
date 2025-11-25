using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DesktopInformationSystem
{
    // Define possible roles in the system
    public enum RoleType { Teacher = 1, Admin = 2, Student = 3 }

    /// <summary>
    /// Abstract base class Person holds common fields and validation.
    /// All user types inherit from this class.
    /// Demonstrates encapsulation.
    /// </summary>
    public abstract class Person
    {
        private int _id;
        private string _name = string.Empty;
        private string _telephone = string.Empty;
        private string _email = string.Empty;
        private RoleType _role;

        // Auto-assigned Id
        public int Id { get => _id; internal set => _id = value; }

        // Name property with validation
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value.Trim();
            }
        }

        // Telephone property with regex validation (Vietnamese phone number)
        public string Telephone
        {
            get => _telephone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Telephone cannot be empty.");
                string s = value.Trim();
                if (!Regex.IsMatch(s, @"^0\d{9,10}$")) // starts with 0, 10-11 digits
                    throw new ArgumentException("Invalid telephone format. Must start with 0 and be 10-11 digits.");
                _telephone = s;
            }
        }

        // Email property with basic regex validation
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");
                string s = value.Trim();
                if (!Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                    throw new ArgumentException("Invalid email format.");
                _email = s;
            }
        }

        // Role property (protected set to prevent external changes)
        public RoleType Role { get => _role; protected set => _role = value; }

        // Constructor to initialize common properties
        protected Person(string name, string tel, string email, RoleType role)
        {
            Name = name;
            Telephone = tel;
            Email = email;
            Role = role;
        }

        // Abstract method to display user information (polymorphism)
        public abstract string ToDisplayString();
    }

    // Teacher class inherits from Person
    public class Teacher : Person
    {
        private decimal _salary;

        public decimal Salary
        {
            get => _salary;
            set
            {
                if (value < 0) throw new ArgumentException("Salary must be >= 0");
                _salary = value;
            }
        }

        public string Subject1 { get; set; }
        public string Subject2 { get; set; }

        public Teacher(string name, string tel, string email, decimal salary, string subject1, string subject2)
            : base(name, tel, email, RoleType.Teacher)
        {
            Salary = salary;
            Subject1 = subject1?.Trim() ?? string.Empty;
            Subject2 = subject2?.Trim() ?? string.Empty;
        }

        public override string ToDisplayString()
        {
            return $"[{Id}] TEACHER | {Name} | {Telephone} | {Email} | Salary: {Salary:C} | Subjects: {Subject1}, {Subject2}";
        }
    }

    // Admin class inherits from Person
    public class Admin : Person
    {
        private decimal _salary;
        private int _workingHours;

        public decimal Salary
        {
            get => _salary;
            set
            {
                if (value < 0) throw new ArgumentException("Salary must be >= 0");
                _salary = value;
            }
        }

        public bool IsFullTime { get; set; }

        public int WorkingHours
        {
            get => _workingHours;
            set
            {
                if (value < 0 || value > 84)
                    throw new ArgumentException("Working hours must be between 0 and 84.");
                _workingHours = value;
            }
        }

        public Admin(string name, string tel, string email, decimal salary, bool isFullTime, int workingHours)
            : base(name, tel, email, RoleType.Admin)
        {
            Salary = salary;
            IsFullTime = isFullTime;
            WorkingHours = workingHours;
        }

        public override string ToDisplayString()
        {
            return $"[{Id}] ADMIN   | {Name} | {Telephone} | {Email} | Salary: {Salary:C} | {(IsFullTime ? "Full-time" : "Part-time")} | {WorkingHours}h/week";
        }
    }

    // Student class inherits from Person
    public class Student : Person
    {
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
        public string Subject3 { get; set; }

        public Student(string name, string tel, string email, string s1, string s2, string s3)
            : base(name, tel, email, RoleType.Student)
        {
            Subject1 = s1?.Trim() ?? string.Empty;
            Subject2 = s2?.Trim() ?? string.Empty;
            Subject3 = s3?.Trim() ?? string.Empty;
        }

        public override string ToDisplayString()
        {
            return $"[{Id}] STUDENT | {Name} | {Telephone} | {Email} | Subjects: {Subject1}, {Subject2}, {Subject3}";
        }
    }

    // DataStore holds dynamic list of Person objects
    public class DataStore
    {
        private readonly List<Person> _people = new(); // List stores unknown number of objects
        private int _nextId = 1; // Auto-increment Id

        // Return all records
        public IEnumerable<Person> GetAll() => _people;

        // Return records by role
        public IEnumerable<Person> GetByRole(RoleType role) => _people.Where(p => p.Role == role);

        // Find a record by Id
        public Person? FindById(int id) => _people.FirstOrDefault(p => p.Id == id);

        // Add new person and assign Id
        public void Add(Person person)
        {
            person.Id = _nextId++;
            _people.Add(person);
        }

        // Delete record by Id
        public bool Delete(int id)
        {
            var p = FindById(id);
            if (p == null) return false;
            _people.Remove(p);
            return true;
        }
    }

    // Input helper class handles user input validation
    public static class Input
    {
        // Read non-empty string
        public static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        // Read and validate email
        public static string ReadEmail(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("Email cannot be empty.");
                    continue;
                }

                if (!Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Invalid email format. Try again.");
                    continue;
                }

                return s;
            }
        }

        // Read and validate telephone number
        public static string ReadTelephone(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("Telephone cannot be empty.");
                    continue;
                }

                if (!Regex.IsMatch(s, @"^0\d{9,10}$"))
                {
                    Console.WriteLine("Invalid telephone format. Must start with 0 and be 10-11 digits.");
                    continue;
                }

                return s;
            }
        }

        // Read decimal number within range
        public static decimal ReadDecimal(string prompt, decimal min = 0, decimal max = 1_000_000_000)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out var v) && v >= min && v <= max)
                    return v;
                Console.WriteLine($"Enter valid number ({min}..{max}).");
            }
        }

        // Read integer within range
        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var v) && v >= min && v <= max)
                    return v;
                Console.WriteLine($"Enter a number between {min} and {max}.");
            }
        }

        // Read yes/no input
        public static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine()?.Trim().ToLower();
                if (s == "y" || s == "yes") return true;
                if (s == "n" || s == "no") return false;
                Console.WriteLine("Please enter Y or N.");
            }
        }

        // Read role selection
        public static RoleType ReadRole()
        {
            Console.WriteLine("Select role: 1.Teacher 2.Admin 3.Student");
            int r = ReadInt("Choice (1-3): ", 1, 3);
            return (RoleType)r;
        }
    }

    // UI class handles menu and user interaction
    public static class UI
    {
        private static readonly DataStore store = new(); // Data storage

        // Main loop
        public static void Run()
        {
            Seed(); // Add sample data

            while (true)
            {
                try
                {
                    ShowMenu();
                    int choice = Input.ReadInt("Select (0-6): ", 0, 6);
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 0: return; // Exit
                        case 1: AddRecord(); break;
                        case 2: ListAll(); break;
                        case 3: ListByRole(); break;
                        case 4: EditRecord(); break;
                        case 5: DeleteRecord(); break;
                        case 6: Help(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        // Display main menu
        private static void ShowMenu()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("  DESKTOP INFORMATION SYSTEM (COMP1551)");
            Console.WriteLine("============================================");
            Console.WriteLine("1. Add new record");
            Console.WriteLine("2. View all records");
            Console.WriteLine("3. View by role");
            Console.WriteLine("4. Edit record");
            Console.WriteLine("5. Delete record");
            Console.WriteLine("6. Help");
            Console.WriteLine("0. Exit");
            Console.WriteLine("--------------------------------------------");
        }

        // Add new record
        private static void AddRecord()
        {
            var role = Input.ReadRole();
            string name = Input.ReadNonEmpty("Name: ");
            string tel = Input.ReadTelephone("Telephone: ");
            string email = Input.ReadEmail("Email: ");

            switch (role)
            {
                case RoleType.Teacher:
                    decimal tSal = Input.ReadDecimal("Salary: ");
                    string t1 = Input.ReadNonEmpty("Subject 1: ");
                    string t2 = Input.ReadNonEmpty("Subject 2: ");
                    store.Add(new Teacher(name, tel, email, tSal, t1, t2));
                    break;

                case RoleType.Admin:
                    decimal aSal = Input.ReadDecimal("Salary: ");
                    bool full = Input.ReadYesNo("Full-time? (Y/N): ");
                    int hours = Input.ReadInt("Working hours/week: ", 0, 84);
                    store.Add(new Admin(name, tel, email, aSal, full, hours));
                    break;

                case RoleType.Student:
                    string s1 = Input.ReadNonEmpty("Subject 1: ");
                    string s2 = Input.ReadNonEmpty("Subject 2: ");
                    string s3 = Input.ReadNonEmpty("Subject 3: ");
                    store.Add(new Student(name, tel, email, s1, s2, s3));
                    break;
            }

            Console.WriteLine("Record added successfully.");
        }

        // List all records
        private static void ListAll()
        {
            var list = store.GetAll().ToList();
            if (!list.Any()) Console.WriteLine("(No data)");
            else list.ForEach(p => Console.WriteLine(p.ToDisplayString()));
        }

        // List records by role
        private static void ListByRole()
        {
            var role = Input.ReadRole();
            var list = store.GetByRole(role).ToList();
            if (!list.Any()) Console.WriteLine("(No records)");
            else list.ForEach(p => Console.WriteLine(p.ToDisplayString()));
        }

        // Edit record
        private static void EditRecord()
        {
            int id = Input.ReadInt("Enter ID to edit: ", 1, int.MaxValue);
            var p = store.FindById(id);
            if (p == null) { Console.WriteLine("Record not found."); return; }

            Console.WriteLine("Current: " + p.ToDisplayString());
            Console.WriteLine("Press Enter to keep value.");

            Console.Write("Name (" + p.Name + "): ");
            string? n = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(n))
            {
                try { p.Name = n.Trim(); }
                catch (ArgumentException ex) { Console.WriteLine("Invalid name: " + ex.Message); }
            }

            Console.Write("Telephone (" + p.Telephone + "): ");
            string? t = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(t))
            {
                try { p.Telephone = Input.ReadTelephone("New Telephone: "); }
                catch (ArgumentException ex) { Console.WriteLine("Invalid telephone: " + ex.Message); }
            }

            Console.Write("Email (" + p.Email + "): ");
            string? e = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(e))
            {
                p.Email = Input.ReadEmail("New Email: ");
            }

            // Role-specific editing
            if (p is Teacher te)
            {
                Console.Write("Salary (" + te.Salary + "): ");
                var sSalary = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(sSalary) && decimal.TryParse(sSalary, out var newSal))
                {
                    try { te.Salary = newSal; }
                    catch (ArgumentException ex) { Console.WriteLine("Invalid salary: " + ex.Message); }
                }

                Console.Write("Subject 1 (" + te.Subject1 + "): ");
                var ns1 = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ns1)) te.Subject1 = ns1.Trim();

                Console.Write("Subject 2 (" + te.Subject2 + "): ");
                var ns2 = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ns2)) te.Subject2 = ns2.Trim();
            }
            else if (p is Admin ad)
            {
                Console.Write("Salary (" + ad.Salary + "): ");
                var sSalary = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(sSalary) && decimal.TryParse(sSalary, out var newSal))
                {
                    try { ad.Salary = newSal; }
                    catch (ArgumentException ex) { Console.WriteLine("Invalid salary: " + ex.Message); }
                }

                Console.Write("Full-time (" + (ad.IsFullTime ? "Y" : "N") + ") [Y/N]: ");
                var ft = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ft))
                {
                    var s = ft.Trim().ToLowerInvariant();
                    if (s.StartsWith("y")) ad.IsFullTime = true;
                    else if (s.StartsWith("n")) ad.IsFullTime = false;
                }

                Console.Write("Working hours (" + ad.WorkingHours + "): ");
                var wh = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(wh) && int.TryParse(wh, out var newWh))
                {
                    try { ad.WorkingHours = newWh; }
                    catch (ArgumentException ex) { Console.WriteLine("Invalid working hours: " + ex.Message); }
                }
            }
            else if (p is Student st)
            {
                Console.Write("Subject 1 (" + st.Subject1 + "): ");
                var xs1 = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(xs1)) st.Subject1 = xs1.Trim();

                Console.Write("Subject 2 (" + st.Subject2 + "): ");
                var xs2 = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(xs2)) st.Subject2 = xs2.Trim();

                Console.Write("Subject 3 (" + st.Subject3 + "): ");
                var xs3 = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(xs3)) st.Subject3 = xs3.Trim();
            }

            Console.WriteLine("Record updated.");
        }

        // Delete record
        private static void DeleteRecord()
        {
            int id = Input.ReadInt("Enter ID to delete: ", 1, int.MaxValue);
            Console.WriteLine(store.Delete(id) ? "Deleted successfully." : "Record not found.");
        }

        // Help information
        private static void Help()
        {
            Console.WriteLine("HELP:");
            Console.WriteLine("• Email must be a valid format (example: user@example.com).");
            Console.WriteLine("• Telephone must start with 0 and be 10-11 digits.");
            Console.WriteLine("• Salary ≥ 0, working hours 0–84.");
            Console.WriteLine("• Use the ID in brackets for edit/delete.");
            Console.WriteLine("• Data is stored in memory only and will be lost when program exits.");
        }

        // Seed sample data
        private static void Seed()
        {
            store.Add(new Teacher("Alice Smith", "0901234567", "alice@school.edu", 1500, "Math", "Physics"));
            store.Add(new Admin("Bob Tran", "0912345678", "bob@school.edu", 1200, true, 40));
            store.Add(new Student("Charlie Le", "0987654321", "charlie@student.edu", "English", "History", "Biology"));
        }
    }

    // Program entry point
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            UI.Run();
        }
    }
}