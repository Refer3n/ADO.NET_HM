using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_Hm3
{
    public class CountriesDatabase
    { 
        private static DbProviderFactory factory = null;

        private static string GetSelectQuery(string tbName) => $"select * from {tbName}";

        Dictionary<string, Dictionary<string, List<string>>> _database;

        private static string Countries => "[dbo].[Countries]";
        private static string Cities => "[dbo].[Cities]";

        public CountriesDatabase()
        {
            DbProviderFactories.RegisterFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName,
                SqlClientFactory.Instance);
            factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);

            _database = new();
            FillDB();
        }

        public string GetAllInfo()
        {
            var allCountryInfo = _database[Countries]["Id"]
                .Select(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);

                    var capitalCityId = GetCountryCapitalId(countryId);
                    var capitalInfo = GetCityInfo(capitalCityId);
                    var citiesInfo = GetCountryCities(countryId).Select(cityId => GetCityInfo(cityId));

                    return $"{countryInfo}\nCapital: {capitalInfo}\nCities:\n\t{string.Join("\n\t", citiesInfo)}";
                })
                .ToList();

            return string.Join("\n", allCountryInfo);
        }

        public string GetCountryNames()
        {
            var countryNames = _database[Countries]["CountryName"].ToList();

            return string.Join("\n", countryNames);
        }

        public string GetCapitalNames()
        {
            var capitalNames = _database[Countries]["Id"]
                .Select(countryId =>
                {
                    var capitalCityId = GetCountryCapitalId(countryId);
                    var capitalInfo = GetCityInfo(capitalCityId);

                    int commaIndex = capitalInfo.IndexOf(',');
                    return capitalInfo.Substring(0, commaIndex).Trim();
                })
                .ToList();

            return string.Join("\n", capitalNames);
        }

        public string GetCitiesByCountry(string countryName)
        {
            var countryNames = _database[Countries]["CountryName"];
            var countryIntId = countryNames.IndexOf(countryName) + 1;
            var countryId = countryIntId.ToString();

            var citiesInfo = GetCountryCities(countryId).Select(cityId => GetCityInfo(cityId));

            return string.Join("\n", citiesInfo);
        }

        public string GetCapitalsWithPopulationOverFiveMillion()
        {
            var capitalNames = _database[Countries]["Id"]
                .Select(countryId =>
                {
                    var capitalCityId = GetCountryCapitalId(countryId);
                    var capitalPopulation = GetPopulationOfCity(capitalCityId);

                    if (capitalPopulation > 5000000)
                    {
                        return GetCityInfo(capitalCityId);
                    }

                    return string.Empty;
                })
                .Where(cityName => !string.IsNullOrEmpty(cityName))
                .ToList();

            return string.Join("\n", capitalNames);
        }

        public string GetEuropeanCountries()
        {
            var europeanCountries = _database[Countries]["Id"]
                .Where(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);
                    return countryInfo.Contains("Continent: Europe");
                })
                .Select(countryId => _database[Countries]["CountryName"][int.Parse(countryId) - 1])
                .ToList();

            return string.Join("\n", europeanCountries);
        }

        public string GetCountriesWithAreaGreaterThan(double minArea)
        {
            return GetCountriesWithAreaInRange(minArea, double.MaxValue);
        }

        public string GetCapitalNamesContainingAI()
        {
            string capitalNames = GetCapitalNames();

            var parsedCapitalNames = capitalNames
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(capitalName => capitalName.Contains("a", StringComparison.OrdinalIgnoreCase)
                 && capitalName.Contains("i", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return string.Join("\n", parsedCapitalNames);
        }

        public string GetCountryNamesContainingK()
        {
            var countryNames = GetCountryNames();

            var parsedCountryNames = countryNames
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(countryName => countryName.Contains("k", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return string.Join("\n", parsedCountryNames);
        }

        public string GetCountriesWithAreaInRange(double minArea, double maxArea)
        {
            var countriesWithAreaGreaterThan = _database[Countries]["Id"]
                .Where(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);

                    string areaString = GetCountryDataFromCountryInfo(countryInfo, 3);

                    double.TryParse(areaString, out double area);

                    return area >= minArea && area <= maxArea;
                })
                .Select(countryId => _database[Countries]["CountryName"][int.Parse(countryId) - 1])
                .ToList();

            return string.Join("\n", countriesWithAreaGreaterThan);
        }

        public string GetCountriesWithPopulationGreaterThan(int minPopulation)
        {
            var countriesWithAreaGreaterThan = _database[Countries]["Id"]
               .Where(countryId =>
               {
                   var countryInfo = GetCountryInfo(countryId);

                   string populationString = GetCountryDataFromCountryInfo(countryInfo, 2);
                   int.TryParse(populationString, out int population);

                   return population >= minPopulation;
               })
               .Select(countryId => _database[Countries]["CountryName"][int.Parse(countryId) - 1])
               .ToList();

            return string.Join("\n", countriesWithAreaGreaterThan);
        }

        public string GetTop5CountriesByArea()
        {
            var countryAreas = _database[Countries]["Id"]
                .Select(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);

                    string areaString = GetCountryDataFromCountryInfo(countryInfo, 3);
                    double area = double.Parse(areaString);

                    string countryName = GetCountryDataFromCountryInfo(countryInfo, 1);

                    return new Tuple<string, double>(countryName, area);
                })
                .ToList();


            var top5Countries = countryAreas
                .OrderByDescending(tuple => tuple.Item2)
                .Take(5)
                .Select(tuple => tuple.Item1)
                .ToList();

            return string.Join("\n", top5Countries);
        }

        public string GetTop5CapitalsByPopulation()
        {
            var capitalInfoList = _database[Countries]["Id"]
                .Select(countryId =>
                {
                    var capitalCityId = GetCountryCapitalId(countryId);
                    return new
                    {
                        CapitalName = GetCityInfo(capitalCityId),
                        Population = GetPopulationOfCity(capitalCityId)
                    };
                })
                .ToList();

            var top5Capitals = capitalInfoList
                .OrderByDescending(capitalInfo => capitalInfo.Population)
                .Take(5)
                .Select(capitalInfo => capitalInfo.CapitalName)
                .ToList();

            return string.Join("\n", top5Capitals);
        }

        public string GetCountryWithLargestArea()
        {
            var top5CountriesByArea = GetTop5CountriesByArea();

            var countries = top5CountriesByArea.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return countries.First();    
        }

        public string GetCapitalWithLargestPopulation()
        {
            var top5CapitalsByArea = GetTop5CapitalsByPopulation();

            var capitals = top5CapitalsByArea.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return capitals.First();
        }

        public string GetCountryWithSmallestAreaInEurope()
        {
            var europeanCountries = GetEuropeanCountries();

            var countryAreas = _database[Countries]["Id"]
                .Where(countryId => europeanCountries.Contains(_database[Countries]["CountryName"][int.Parse(countryId) - 1]))
                .Select(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);

                    string countryName = GetCountryDataFromCountryInfo(countryInfo, 1);
                    string areaString = GetCountryDataFromCountryInfo(countryInfo, 3);
                    double area = double.Parse(areaString);

                    return new Tuple<string, double>(countryName, area);
                })
                .OrderBy(tuple => tuple.Item2)
                .First();

            return countryAreas.Item1;
        }

        public string GetAverageAreaOfEuropeanCountries()
        {
            var europeanCountries = GetEuropeanCountries();
            var europeanCountryNames = europeanCountries.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var totalArea = _database[Countries]["Id"]
                .Where(countryId => europeanCountries.Contains(_database[Countries]["CountryName"][int.Parse(countryId) - 1]))
                .Select(countryId =>
                {
                    var countryInfo = GetCountryInfo(countryId);

                    string areaString = GetCountryDataFromCountryInfo(countryInfo, 3);
                    return double.Parse(areaString);
                })
                .Sum();

            return (totalArea / europeanCountryNames.Count()).ToString();
        }

        public string GetTop3CitiesByPopulation(string countryName)
        {
            var countryNames = _database[Countries]["CountryName"];
            var countryIndex = countryNames.IndexOf(countryName) + 1;

            var countryId = countryIndex.ToString();
            var cityIds = GetCountryCities(countryId);

            var cityPopulationList = cityIds
                .Select(cityId =>
                {
                    var cityInfo = GetCityInfo(cityId);
                    int population = GetPopulationOfCity(cityId);
                    return new { cityInfo, Population = population };
                })
                .OrderByDescending(city => city.Population)
                .Take(3)
                .Select(city => city.cityInfo)
                .ToList();

            return string.Join("\n", cityPopulationList);
        }

        public string GetTotalCountriesCount()
        {
            return _database[Countries]["Id"].Count().ToString();
        }

        public string GetContinentWithMostCountries()
        {
            var continentCounts = _database[Countries]["Continent"]
                .GroupBy(continent => continent)
                .Select(group => new { Continent = group.Key, Count = group.Count() })
                .OrderByDescending(group => group.Count)
                .FirstOrDefault();

            return $"{continentCounts.Continent} ({continentCounts.Count} countries)";
        }

        public string GetCountriesCountByContinent()
        {
            var continentCounts = _database[Countries]["Continent"]
                .GroupBy(continent => continent)
                .Select(group => new { Continent = group.Key, Count = group.Count() })
                .OrderBy(group => group.Continent)
                .ToList();

            var result = continentCounts.Select(group => $"{group.Continent}: {group.Count} countries");
            return string.Join("\n", result);
        }

        private int GetPopulationOfCity(int cityId)
        {
            int population = 0;
            var populationString = _database[Cities]["Population"][cityId - 1];
            int.TryParse(populationString, out population);
            return population;
        }

        private string GetCountryDataFromCountryInfo(string countryInfo, int index)
        {
            var result = countryInfo.Split(':')[index].Trim();
            return result.Substring(0, result.IndexOf(','));
        }

        private string GetCountryInfo(string countryId)
        {
            var intCountryId = Convert.ToInt32(countryId) - 1;

            string name = _database[Countries]["CountryName"][intCountryId];
            string population = _database[Countries]["Population"][intCountryId];
            string area = _database[Countries]["Area"][intCountryId];
            string continent = _database[Countries]["Continent"][intCountryId];

            return $"CountryName: {name}, Population: {population}, Area: {area}, Continent: {continent}";
        }

        private int GetCountryCapitalId(string countryId)
        {
            return _database[Cities]["IsCapital"]
                .Select((value, index) => new { Value = value, Index = index })
                .Where(item => item.Value == "True" && _database[Cities]["CountryID"].ElementAtOrDefault(item.Index) == countryId)
                .Select(item => int.Parse(_database[Cities]["Id"][item.Index]))
                .First();
        }

        private string GetCityInfo(int cityId)
        {
            cityId--;
            string cityName = _database[Cities]["CityName"][cityId];
            string population = _database[Cities]["Population"][cityId];

            return $"{cityName}, Population: {population}";
        }

        private List<int> GetCountryCities(string countryId)
        {
            return _database[Cities]["CountryID"]
                .Select((value, index) => new { Value = value, Index = index })
                .Where(item => item.Value == countryId)
                .Select(item => int.Parse(_database[Cities]["Id"][item.Index]))
                .ToList();
        }

        private void FillDB()
        {
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

                connection.Open();

                var tableNames = GetTableNames(connection);

                foreach (var tableName in tableNames)
                {
                    var columns = GetColumnNames(connection, tableName);

                    var tableData = new Dictionary<string, List<string>>();

                    using (DbCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = GetSelectQuery(tableName);

                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                foreach (var columnName in columns)
                                {
                                    if (!tableData.ContainsKey(columnName))
                                    {
                                        tableData[columnName] = new List<string>();
                                    }
                                    tableData[columnName].Add(reader[columnName].ToString());
                                }
                            }
                        }
                    }

                    _database[tableName] = tableData;
                }

                connection.Close();
            }
        }

        private static List<string> GetTableNames(DbConnection connection, params string[] name)
        {
            var tables = new List<string>();

            if (name.Length > 0)
            {
                foreach (DataRow dr in connection.GetSchema("Tables").Rows)
                {
                    if (name.Contains(dr[2].ToString()))
                    {
                        tables.Add($"[{dr[1]}].[{dr[2]}]");
                    }
                }
            }
            else
            {
                foreach (DataRow dr in connection.GetSchema("Tables").Rows)
                {
                    tables.Add($"[{dr[1]}].[{dr[2]}]");
                }
            }

            return tables;
        }

        private static List<string> GetColumnNames(DbConnection connection, string tbName)
        {
            var colums = new List<string>();

            using (DbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = GetSelectQuery(tbName);
                
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        colums.Add(reader.GetName(i));
                    }
                }
            }
            return colums;
        }
    }
}
