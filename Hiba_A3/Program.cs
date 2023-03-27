using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hiba_A3
{
    //hiba syed
    //301318542
    internal class Program
    {
        static void Main(string[] args)
        {
            TestAccounts();
        }

        static void TestAccounts()
        {
            Bank.AccountList.Add(new SavingsAccount("S647", "Alex Du", 222290192, 4783.98));
            Bank.AccountList.Add(new ChequingAccount("C576", "Dale Stayne", 333312312, 12894.56));

            Bank.ShowAll();

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to withdraw $500.00 from the following account");
            Console.WriteLine(Bank.AccountList[0].ToString());
            Bank.AccountList[0].Withdraw(500.00);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to deposit $-250.00 to the following account");
            Console.WriteLine(Bank.AccountList[1].ToString());
            Bank.AccountList[1].Deposit(-250.00);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to withdraw $10000.00 to the following account");
            Console.WriteLine(Bank.AccountList[2].ToString());
            Bank.AccountList[2].Withdraw(10000.00);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to deposit $5000.00 to the following account");
            Console.WriteLine(Bank.AccountList[2].ToString());
            Bank.AccountList[2].Deposit(5000.00);
            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to deposit $7300.00 to the following account");
            Console.WriteLine(Bank.AccountList[3].ToString());
            Bank.AccountList[3].Deposit(7300.90);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to withdraw $45000.40 from the following account");
            Console.WriteLine(Bank.AccountList[4].ToString());
            Bank.AccountList[4].Withdraw(45000.40);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Console.WriteLine("Trying to withdraw $37000.00 from the following account");
            Console.WriteLine(Bank.AccountList[5].ToString());
            Bank.AccountList[5].Withdraw(37000);
            Console.WriteLine($"{new string('-', 90)}");

            Console.WriteLine($"{new string('-', 90)}");
            Bank.ShowAll(67676767);
            Console.WriteLine($"{new string('-', 90)}");

            Bank.ShowAll();
        }

        public class Consumer
        {
            public string id { get; }
            public string name { get; }

            public Consumer(string id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public override string ToString()
            {
                return $"ID: {id}, Name: {name}";
            }

        }
        public abstract class Account : Consumer
        {
            public int AccountNum { get; }

            public Account(string id, string name, int accountNum) : base(id, name)
            {
                this.AccountNum = accountNum;
            }
            public abstract void Withdraw(double amount);

            public abstract void Deposit(double amount);

            public override string ToString()
            {
                return $"{base.ToString()}, Account Number: {AccountNum}";
            }
        }
        public class InsufficientBalanceException : Exception
        {
            public InsufficientBalanceException() : base("Account not having enough balance to withdraw.")
            {
            }
        }

        // MinimumBalanceException
        public class MinimumBalanceException : Exception
        {
            public MinimumBalanceException() : base("You must maintain minimum $3000 balance in Saving account.")
            {
            }
        }

        // IncorrectAmountException
        public class IncorrectAmountException : Exception
        {
            public IncorrectAmountException() : base("You must provide positive number for amount to be deposited.")
            {
            }
        }

        // OverdraftLimitException
        public class OverdraftLimitException : Exception
        {
            public OverdraftLimitException() : base("Overdraft Limit exceeded. You can only use $2000 from overdraft.")
            {
            }
        }

        // AccountNotFoundException
        public class AccountNotFoundException : Exception
        {
            public AccountNotFoundException() : base("Account with given number does not exist.")
            {
            }
        }

        public class SavingsAccount : Account
        {
            public double Balance { get; set; }


            public SavingsAccount(string id, string name, int accountNum, double balance = 0.0) : base(id, name, accountNum)
            {
                this.Balance = balance;
            }

            public override void Withdraw(double amount)
            {
                try
                {
                    if (amount <= 0)
                    {
                        throw new IncorrectAmountException();
                    }
                    if (Balance < amount)
                    {
                        throw new InsufficientBalanceException();
                    }
                    if (Balance - amount < 3000)
                    {
                        throw new MinimumBalanceException();
                    }
                    Balance -= amount;
                    Console.WriteLine("Updated balance: {0}", Balance);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }

            public override void Deposit(double amount)
            {
                try
                {
                    if (amount <= 0)
                    {
                        throw new IncorrectAmountException();
                    }
                    Balance += amount;
                    Console.WriteLine("Updated balance: {0}", Balance);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
            public override string ToString()
            {
                return string.Format("Account Number: {0}, Consumer ID: {1}, Consumer Name: {2}, Balance: {3}",
                    AccountNum, id, name, Balance);
            }
        }

        public class ChequingAccount : Account
        {
            public double Balance { get; set; }
            public ChequingAccount(string id, string name, int accountNum, double balance = 0.0)
            : base(id, name, accountNum)
            {
                this.Balance = balance;
            }

            public override void Withdraw(double amount)
            {
                double overdraftLimit = Balance + 2000;
                if (amount > overdraftLimit)
                {
                    throw new OverdraftLimitException();
                }
                else
                {
                    Balance -= amount;
                    Console.WriteLine($"Updated balance is {this.Balance:C}.");
                }
            }

            public override void Deposit(double amount)
            {
                if (amount < 0)
                {
                    throw new IncorrectAmountException();
                }
                else
                {
                    Balance += amount;
                    Console.WriteLine($"Updated balance is {this.Balance:C}.");
                }
            }

            public override string ToString()
            {
                return base.ToString() + $" Chequing Balance: {this.Balance:C}";
            }
        }

        public class Bank
        {
            public static List<Account> AccountList;

            static Bank()
            {
                AccountList = new List<Account>();
                AccountList.Add(new SavingsAccount("S101", "James Finch", 222210212, 3400.90));
                AccountList.Add(new SavingsAccount("S102", "Kell Forest", 222214500, 42520.32));
                AccountList.Add(new SavingsAccount("S103", "Amy Stone", 222212000, 8273.45));
                AccountList.Add(new ChequingAccount("C104", "Kaitlin Ross", 333315002, 91230.45));
                AccountList.Add(new ChequingAccount("C105", "Adem First", 333303019, 43987.67));
                AccountList.Add(new ChequingAccount("C106", "John Doe", 333358927, 34829.76));
            }

            public static void ShowAll()
            {
                foreach (Account account in AccountList)
                {
                    Console.WriteLine(account);
                    Console.WriteLine();
                }
            }

            public static void ShowAll(int accountNum)
            {
                foreach (Account account in AccountList)
                {
                    if (account.AccountNum == accountNum)
                    {
                        Console.WriteLine(account);
                        Console.WriteLine();
                    }
                    else
                    {
                        throw new AccountNotFoundException();
                    }
                }
            }


        }





    }
}
