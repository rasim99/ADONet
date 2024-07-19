using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONet.Constants
{
    public enum Operations
    {
        Exit,
        AllCountries,
        AddCountry,
        UpdateCountry,
        DeleteCountry,
        DetailsOfCountry,
        AllCities,
        AllCitiesOfCountry,
        AddCity,
        UpdateCity,
        DeleteCity,
        DetailsOfCity
    }

    public enum CrudOperationType
    {
        Add,
        Update,
        Delete
    }
}
