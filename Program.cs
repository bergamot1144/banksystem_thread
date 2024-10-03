using System;
using System.Threading;

class BankAccount
{
    private decimal balance;
    private readonly object balanceLock = new object(); 

    public BankAccount(decimal initialBalance)
    {
        balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        lock (balanceLock) // Синхронизация на уровне метода
        {
            balance += amount;
            Console.WriteLine($"{Thread.CurrentThread.Name} внес {amount:C}. Баланс: {balance:C}");
        }
    }

    public void Withdraw(decimal amount)
    {
        lock (balanceLock) // Синхронизация на уровне метода
        {
            if (balance >= amount)
            {
                balance -= amount;
                Console.WriteLine($"{Thread.CurrentThread.Name} снял {amount:C}. Баланс: {balance:C}");
            }
            else
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} попытался снять {amount:C}, но недостаточно средств. Баланс: {balance:C}");
            }
        }
    }

    public decimal GetBalance()
    {
        lock (balanceLock)
        {
            return balance;
        }
    }
}

class Program
{
    private static Random random = new Random();

    static void Main()
    {
        BankAccount account = new BankAccount(1000); 
        Thread[] clients = new Thread[5]; 

        for (int i = 0; i < clients.Length; i++)
        {
            clients[i] = new Thread(PerformRandomOperations)
            {
                Name = $"Клиент {i + 1}"
            };
            clients[i].Start(account);
        }

        foreach (Thread client in clients)
        {
            client.Join(); 
        }

        Console.WriteLine($"Финальный баланс: {account.GetBalance():C}");
    }

    static void PerformRandomOperations(object obj)
    {
        BankAccount account = (BankAccount)obj;
        DateTime end = DateTime.Now.AddSeconds(10); 
        while (DateTime.Now < end)
        {
            int operation = random.Next(2); 
            decimal amount = random.Next(1, 500); 

            if (operation == 0)
            {
                account.Withdraw(amount);
            }
            else
            {
                account.Deposit(amount);
            }

            Thread.Sleep(random.Next(100, 500)); 
        }
    }
}
