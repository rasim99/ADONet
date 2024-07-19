using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONet.Constants
{
    public static class Messages
    {
        public static void InvalidInputMessage(string title) => Console.WriteLine($"{title} is invalid!! Please try again ");
        public static void InputMessage(string title) => Console.WriteLine($"Input {title}");
        public static void SuccesseMessage(string title,string value,CrudOperationType type) => Console.WriteLine($"{title} - {value} Succesfuly " +
            $"{(type==CrudOperationType.Add? "Added":type==CrudOperationType.Update?"Updated":type==CrudOperationType.Delete?"Deleted":" ")}");
        public static void ErrorOcuredMessage() => Console.WriteLine("Error ocured.please try again!");
        public static void AlreadyExistMessage(string title, string value) => Console.WriteLine($"{title} - {value} already exist");
        public static void PrintMessage(string title, string value) => Console.WriteLine($"{title} - {value} ");
        public static void NotFoundMessage(string title, string value) => Console.WriteLine($"{title} - {value} not found! ");
        public static void PrintWantToChangeMessage(string title) => Console.WriteLine($" Do you want to change {title} ? Yes or Not?");

    }
}
