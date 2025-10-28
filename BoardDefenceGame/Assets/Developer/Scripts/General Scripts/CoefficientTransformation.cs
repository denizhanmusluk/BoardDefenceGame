using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public static class CoefficientTransformation
{
    public static string Converter(int number)
    {
        int integerPart;
        int decimalPart;
        string result = number.ToString();
       if( number >= 1000)
        {
            integerPart = number / 1000;
            decimalPart = number % 1000;
            if(decimalPart >= 100)
            {

                if(number >= 10000)
                {
                    decimalPart /= 100;
                    result = integerPart.ToString() + "." + decimalPart.ToString() + "K";
                }
                else
                {
                    decimalPart /= 10;
                    result = integerPart.ToString() + "." + decimalPart.ToString() + "K";
                }
            }
            else
            {
                if (number >= 10000)
                {
                    result = integerPart.ToString() + ".0K";
                }
                else
                {
                    decimalPart /= 10;
                    result = integerPart.ToString() + ".0" + decimalPart.ToString() + "K";
                }

            }
        }
        return result;
    }
}
