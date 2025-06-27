using System.Collections;
using System.Globalization;

namespace VideoStore;

public class Customer
{
    private static readonly CultureInfo UsCultureInfo = new("en-US");
    private readonly string _name;
    private readonly ArrayList _rentals = new();

    public Customer(string name)
    {
        _name = name;
    }

    public void AddRental(Rental rental)
    {
        _rentals.Add(rental);
    }

    public string GetName()
    {
        return _name;
    }

    public string Statement()
    {
        double totalAmount = 0;
        var frequentRenterPoints = 0;
        var result = "Rental Record for " + GetName() + "\n";

        foreach (Rental each in _rentals)
        {
            double thisAmount = 0;

            // determines the amount for each line
            switch (each.GetMovie().GetPriceCode())
            {
                case Movie.Regular:
                    thisAmount += 2;
                    if (each.GetDaysRented() > 2)
                        thisAmount += (each.GetDaysRented() - 2) * 1.5;
                    break;
                case Movie.NewRelease:
                    thisAmount += each.GetDaysRented() * 3;
                    break;
                case Movie.Children:
                    thisAmount += 1.5;
                    if (each.GetDaysRented() > 3)
                        thisAmount += (each.GetDaysRented() - 3) * 1.5;
                    break;
            }

            frequentRenterPoints++;

            if (each.GetMovie().GetPriceCode() == Movie.NewRelease
                && each.GetDaysRented() > 1)
                frequentRenterPoints++;

            result += "\t" + each.GetMovie().GetTitle() + "\t"
                      + string.Format(UsCultureInfo, "{0:0.0}", thisAmount) + "\n";
            totalAmount += thisAmount;
        }

        result += "You owed " + string.Format(UsCultureInfo, "{0:0.0}", totalAmount) + "\n";
        result += "You earned " + frequentRenterPoints + " frequent renter points\n";

        return result;
    }
}