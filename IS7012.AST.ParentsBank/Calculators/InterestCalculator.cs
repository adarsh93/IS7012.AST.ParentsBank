using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace IS7012.AST.ParentsBank.Calculators
{
    public class InterestTransaction
    {
        public DateTime date;
        public decimal amount;

        public InterestTransaction(DateTime date, decimal amount)
        {
            this.date = date;
            this.amount = amount;
        }
    }


    class InterestCalculator
    {
        public decimal calculateYTD_Interest(DateTime startDate, DateTime endDate, decimal decIntRate, decimal decStartBal, List<InterestTransaction> lstTransactions)
        {
            //select all of the transactions between the starting and ending dates
            //subtotal amount by date and convert the final item into a dictionary<txn_date,total_txn_amount>
            Dictionary<DateTime, decimal> txns = lstTransactions.Where(it => (it.date >= startDate && it.date <= endDate))
                                      .GroupBy(it => it.date)
                                      .Select(it => new
                                      {
                                          Date = it.Key,
                                          Total = it.Select(txn => txn.amount).Sum()
                                      }).ToDictionary(it => it.Date, it => it.Total);

            decimal decRunningTotal = 0;
            TimeSpan tsDuration = endDate - startDate;
            int iYearToDateDays = tsDuration.Days;
            int iCompoundingTimes = 12;
            int iTotalDaysInYear = (startDate.AddYears(1) - startDate).Days; //calculate the number of days between the starting year and next year.
            decimal timePeriod = 1 / (decimal)iTotalDaysInYear; //calculating daily interest


            //iterate over the date range 
            foreach (DateTime day in EachDay(startDate, endDate))
            {
                decimal decDaysTxnAmt = 0;

                //if the txn is in the dictionary, change the transaction amount.
                if (txns.ContainsKey(day)) { decDaysTxnAmt = txns[day]; Console.WriteLine(decDaysTxnAmt); }

                decRunningTotal += decDaysTxnAmt;
                //   Console.WriteLine("running total: " + decRunningTotal);
                Console.WriteLine("interest : " + decIntRate);

                double principalAndInterest = compoundInterestFormula(decRunningTotal, decIntRate, iCompoundingTimes, timePeriod);
                //   Console.WriteLine(principalAndInterest);
                Console.WriteLine("principal and interest: " + principalAndInterest);

                decRunningTotal = (decimal)principalAndInterest;
            }


            return decRunningTotal;
        }

        private double compoundInterestFormula(decimal principal,
                                               decimal interestRate,
                                               int compoundingTimes,
                                               decimal TimePeriod
                                               )
        {
            double body = (double)(1 + (interestRate / compoundingTimes));
            double exponent = (double)(compoundingTimes * (TimePeriod));

            return (double)principal * Math.Pow(body, exponent);
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}