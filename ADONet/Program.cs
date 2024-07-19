using ADONet.Constants;
using ADONet.Services;
using System.Linq.Expressions;
using System.Threading.Channels;

namespace ADONet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                ShowMenu();
                string choiceInput = Console.ReadLine();
                int choice;
                bool isSuccessed = int.TryParse(choiceInput, out choice);
                if (isSuccessed)
                {
                    switch ((Operations)choice)
                    {
                        case Operations.Exit:
                            return;

                        case Operations.AllCountries:
                            CountryService.GetAllCountries();
                            break;
                        case Operations.AddCountry:
                            CountryService.AddCountry();
                            break;
                        case Operations.UpdateCountry:
                            CountryService.UpdateCountry();
                            break;
                        case Operations.DeleteCountry:
                            CountryService.DeleteCountry();
                            break;
                        case Operations.DetailsOfCountry:
                            CountryService.DetailsOfCountry();
                            break;
                        case Operations.AllCities:
                            CityService.AllCities();
                            break;
                        case Operations.AllCitiesOfCountry:
                            CityService.AllCitiesOfCountry();
                            break;
                        case Operations.AddCity:
                            CityService.AddCity();
                            break;
                        case Operations.UpdateCity:
                            CityService.UpdateCity();
                            break;
                        case Operations.DeleteCity:
                            CityService.DeleteCity();
                            break;
                        case Operations.DetailsOfCity:
                            CityService.DetailsOfCity();
                            break;
                        default:
                            Messages.InvalidInputMessage("Choice");
                            break;
                    }

                }
                else
                {
                    Messages.InvalidInputMessage("Choice");
                }
            }

        }

        private static void ShowMenu()
        {
            Console.WriteLine(" ---    MENU    --- ");
            Console.WriteLine("1 All Countries ");
            Console.WriteLine("2 Add Country ");
            Console.WriteLine("3 Update Country ");
            Console.WriteLine("4 Delete Country");
            Console.WriteLine("5 Details of Country");
            Console.WriteLine("6 All Cities ");
            Console.WriteLine("7 All Cities of Country ");
            Console.WriteLine("8 Add City ");
            Console.WriteLine("9 Update City ");
            Console.WriteLine("10 Delete City ");
            Console.WriteLine("11 Details of City ");
            Console.WriteLine("0 Exit ");
        }
    }
}
