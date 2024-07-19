using ADONet.Constants;
using ADONet.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONet.Services
{
    public static class CityService
    {
        public static void AllCities()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
            {
                connection.Open();
                SqlCommand countCommand = new SqlCommand(" SELECT COUNT(*) FROM Cities WHERE IsDeleted=0 ", connection);
                int cityCount = Convert.ToInt32(countCommand.ExecuteScalar());
                if (cityCount == 0)
                {
                    Messages.NotFoundMessage("Any", "City");
                    return;
                }

                SqlCommand command = new SqlCommand(" SELECT * FROM Cities WHERE IsDeleted=0 ", connection);
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Messages.PrintMessage("Id ", Convert.ToString(reader["Id"]));
                        Messages.PrintMessage("Name ", Convert.ToString(reader["Name"]));
                        Messages.PrintMessage("Area ", Convert.ToString(reader["Area"]));
                        Console.WriteLine();
                    }
                }
            }
        }

        public static void AllCitiesOfCountry()
        {
            using (var conn = new SqlConnection(ConnectionStrings.Default))
            {
                conn.Open();
                var checkCommand = new SqlCommand(" SELECT COUNT(*) FROM Countries WHERE IsDeleted=0", conn);
                int countryCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (countryCount == 0)
                {
                    Messages.NotFoundMessage("Please created country Firstly ", " Because any country");
                    return;
                }
            }
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
            {
                connection.Open();
                CountryService.GetAllCountries();
            CountryNameInput: Messages.InputMessage("Country of City Details");
                string countryName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(countryName))
                {
                    var selectCommand = new SqlCommand(" SELECT * FROM Countries WHERE Name=@name  AND IsDeleted=0", connection);
                    selectCommand.Parameters.AddWithValue("@name", countryName);
                    int countryId = Convert.ToInt32(selectCommand.ExecuteScalar());
                    if (countryId > 0)
                    {
                        var command = new SqlCommand(" SELECT * FROM Cities WHERE CountryId=@id AND IsDeleted=0 ", connection);
                        command.Parameters.AddWithValue("@id", countryId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Messages.PrintMessage("Name", Convert.ToString(reader["Name"]));
                                    Messages.PrintMessage("Area", Convert.ToString(reader["Area"]));
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Messages.NotFoundMessage("Any City on", countryName);
                            }
                        }

                    }
                    else
                    {
                        Messages.NotFoundMessage("Country", countryName);
                        return;
                    }
                }
                else
                {
                    Messages.InvalidInputMessage("Country name");
                    goto CountryNameInput;
                }
            }

        }

        public static void AddCity()
        {
            using (var conn = new SqlConnection(ConnectionStrings.Default))
            {
                conn.Open();
                SqlCommand countCountryCommand = new SqlCommand(" SELECT COUNT(*) FROM Countries WHERE IsDeleted=0 ", conn);
                int countryCount = Convert.ToInt32(countCountryCommand.ExecuteScalar());
                if (countryCount == 0)
                {
                    Messages.NotFoundMessage("Please Create country! ", " Becaause any country");
                    return;
                }
            }

        CounrtyNameInput: CountryService.GetAllCountries();
            Messages.InputMessage("Country name");
            string countryName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(countryName))
            {

                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    SqlCommand selectCommand = new SqlCommand(" SELECT * FROM Countries WHERE Name=@name AND IsDeleted=0", connection);
                    selectCommand.Parameters.AddWithValue("@name", countryName);
                    try
                    {
                        int countryId = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (countryId < 1)
                        {
                            Messages.NotFoundMessage("Country", countryName);
                            goto CounrtyNameInput;
                        }
                    CityNameInput: Messages.InputMessage("City name");
                        string cityName = Console.ReadLine();
                        if (string.IsNullOrEmpty(cityName))
                        {
                            Messages.InvalidInputMessage("city name");
                            goto CityNameInput;
                        }

                        SqlCommand existCityCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name AND CountryId = @id", connection);
                        existCityCommand.Parameters.AddWithValue("@name", cityName);
                        existCityCommand.Parameters.AddWithValue("@id", countryId);
                        var existCityId = Convert.ToInt32(existCityCommand.ExecuteScalar());
                        if (existCityId > 0)
                        {
                            Messages.AlreadyExistMessage("City", cityName);
                            return;
                        }

                    AreaInput: Messages.InputMessage("City Area");
                        string areaInput = Console.ReadLine();
                        decimal area;
                        bool isSuccessed = decimal.TryParse(areaInput, out area);
                        if (!isSuccessed)
                        {
                            Messages.InvalidInputMessage("Area");
                            goto AreaInput;
                        }
                        var command = new SqlCommand("INSERT INTO Cities ( Name , Area,CountryId) VALUES (@name,@area,@countryId)", connection);
                        command.Parameters.AddWithValue("@name", cityName);
                        command.Parameters.AddWithValue("@area", area);
                        command.Parameters.AddWithValue("@countryId", countryId);
                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows > 0)
                        {
                            Messages.SuccesseMessage("City", cityName, CrudOperationType.Add);
                        }
                        else
                        {
                            Messages.ErrorOcuredMessage();
                        }
                    }
                    catch (Exception)
                    {

                        Messages.ErrorOcuredMessage();
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("Country name");
                goto CounrtyNameInput;
            }
        }

        public static void UpdateCity()
        {
            AllCities();
        CityNameInput: Messages.InputMessage(" Id");
            string cityIdInput = Console.ReadLine();
            int cityId;
            bool isSuccessed = int.TryParse(cityIdInput, out cityId);
            if (isSuccessed)
            {
                using (var connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var command = new SqlCommand(" SELECT * FROM Cities WHERE Id=@id AND IsDeleted=0 ", connection);
                    command.Parameters.AddWithValue("@id", cityId);
                    int hasCityId = Convert.ToInt32(command.ExecuteScalar());
                    if (hasCityId > 0)
                    {
                        var countryCommand = new SqlCommand(" SELECT CountryId FROM Cities WHERE Id=@id AND IsDeleted=0 ", connection);
                        countryCommand.Parameters.AddWithValue("@id", cityId);
                        int countryId = Convert.ToInt32(countryCommand.ExecuteScalar());

                    ChangeCityNameInput: Messages.PrintWantToChangeMessage("Name");
                        string choiceInput = Console.ReadLine();
                        char choice;
                        isSuccessed = char.TryParse(choiceInput, out choice);
                        if (isSuccessed && choice.IsValidChoice())
                        {
                            string newName = string.Empty;
                            if (choice.Equals('y'))
                            {
                            NewCityNameInput: Messages.InputMessage("new name");
                                newName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    var existCityCommand = new SqlCommand(" SELECT * FROM Cities WHERE Name=@name AND Id!=@id AND CountryId = @countryId", connection);
                                    existCityCommand.Parameters.AddWithValue("@name", newName);
                                    existCityCommand.Parameters.AddWithValue("@id", cityId);
                                    existCityCommand.Parameters.AddWithValue("@countryId", countryId);
                                    int existId = Convert.ToInt32(existCityCommand.ExecuteScalar());
                                    if (existId > 0)
                                    {
                                        Messages.AlreadyExistMessage("City", newName);
                                        goto ChangeCityNameInput;
                                    }

                                }
                                else
                                {
                                    Messages.InvalidInputMessage("new name");
                                    goto NewCityNameInput;
                                }
                            }

                        ChangeCityAreaInput: Messages.PrintWantToChangeMessage("Area");
                            decimal newArea = default;
                            choiceInput = Console.ReadLine();
                            isSuccessed = char.TryParse(choiceInput, out choice);
                            if (isSuccessed && choice.IsValidChoice())
                            {
                                if (choice.Equals('y'))
                                {
                                    Messages.InputMessage("new area");
                                    string newAreaInput = Console.ReadLine();
                                    isSuccessed = decimal.TryParse(newAreaInput, out newArea);
                                    if (!isSuccessed)
                                    {
                                        Messages.InvalidInputMessage("new area");
                                        goto ChangeCityAreaInput;
                                    }
                                }
                            }
                            else
                            {

                                Messages.InvalidInputMessage("choice");
                                goto ChangeCityAreaInput;
                            }

                            var updateCommand = new SqlCommand("UPDATE Cities SET  ", connection);
                            if (newName != string.Empty || newArea != default)
                            {
                                bool isComma = false;
                                if (newName != string.Empty)
                                {
                                    isComma = true;
                                    updateCommand.CommandText = updateCommand.CommandText + " Name=@name ";
                                    updateCommand.Parameters.AddWithValue("@name", newName);
                                }
                                if (newArea != default)
                                {
                                    updateCommand.CommandText = updateCommand.CommandText + $"{(isComma ? " , " : "")}  Area=@area ";
                                    updateCommand.Parameters.AddWithValue("@area", newArea);
                                }

                                updateCommand.CommandText = updateCommand.CommandText + " WHERE Id=@id";
                                updateCommand.Parameters.AddWithValue("@id", cityId);
                                int affectRows = Convert.ToInt32(updateCommand.ExecuteNonQuery());

                                if (affectRows > 0)
                                {
                                    Messages.SuccesseMessage("City", newName, CrudOperationType.Update);
                                }
                                else
                                    Messages.ErrorOcuredMessage();
                            }

                        }
                        else
                        {
                            Messages.InvalidInputMessage("choice");
                            goto ChangeCityNameInput;
                        }
                    }
                    else
                    {
                        Messages.NotFoundMessage("City", cityIdInput);
                        goto CityNameInput;
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("city Id");
                return;
            }
        }

        public static void DeleteCity()
        {
            AllCities();
        DeleteCityIdInput: Messages.InputMessage("City Id");
            string idInput = Console.ReadLine();
            int id;
            bool isSuccessed = int.TryParse(idInput, out id);
            if (isSuccessed)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(" SELECT * FROM Cities WHERE Id=@id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    if ((Convert.ToInt32(command.ExecuteScalar()) > 0))
                    {
                        SqlCommand deletedCommand = new SqlCommand("UPDATE Cities SET IsDeleted=1  WHERE Id=@id", connection);
                        deletedCommand.Parameters.AddWithValue("@id", id);
                        int affectedRows = deletedCommand.ExecuteNonQuery();
                        if (affectedRows > 0)
                        {
                            Messages.SuccesseMessage("City", $"{id}", CrudOperationType.Delete);
                        }
                        else
                        {
                            Messages.ErrorOcuredMessage();
                        }

                    }
                    else
                    {
                        Messages.NotFoundMessage("City", "Id");
                        return;
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("Id");
                goto DeleteCityIdInput;
            }

        }

        public static void DetailsOfCity()
        {
            AllCities();
        CityIdInput: Messages.InputMessage("City Id");
            string cityIdInput = Console.ReadLine();
            int cityId;
            bool isSuccessed = int.TryParse(cityIdInput, out cityId);
            if (isSuccessed)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();
                    var selectCommand = new SqlCommand("SELECT * FROM Cities WHERE Id=@id AND IsDeleted=0", connection);
                    selectCommand.Parameters.AddWithValue("@id", cityId);
                    int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                    if (id > 0)
                    {
                        var joinnedCommand = new SqlCommand("SELECT ci.Name AS CityName,ci.Area AS CityArea,co.Name AS CountryName from Cities ci " +
                            "JOIN  Countries co " +
                            "ON ci.CountryId=co.Id WHERE ci.Id=@id ", connection);
                        joinnedCommand.Parameters.AddWithValue("@id", id);
                        using (var reader = joinnedCommand.ExecuteReader())
                        {
                            reader.Read();
                            Messages.PrintMessage(" Name ", Convert.ToString(reader["CityName"]));
                            Messages.PrintMessage(" Country  ", Convert.ToString(reader["CountryName"]));
                            Messages.PrintMessage(" Area ", Convert.ToString(reader["CityArea"]));


                        }
                    }
                    else
                    {
                        Messages.NotFoundMessage("city", $"{cityId}");
                        return;
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("Id");
                goto CityIdInput;
            }
        }
    }
}
