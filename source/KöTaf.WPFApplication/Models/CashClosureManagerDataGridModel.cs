using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert die Datensätze im Kassenabschluss-Manager
    /// </summary>
    class CashClosureManagerDataGridModel
    {
        public CashClosure cashClosure { get; set; }
        public CashClosureReport cashClosureReport { get; set; }
        public CashClosureReceipt cashClosureReceipt { get; set; }
        public int cashClosureID { get; set; }
        public string closureDate { get; set; }
        public string closureUser { get; set; }
        public string comment { get; set; }
        public string revenues { get; set; }
        public string expenses { get; set; }
        public string sum { get; set; }
        public bool reportPrinted { get; set; }
        public string reportPrintDate { get; set; }
        public string reportPrintUser { get; set; }
        public bool reportDone { get; set; }
        public bool reportDoneBoxEnabled { get; set; }
        public string reportDoneDate { get; set; }
        public string reportDoneUser { get; set; }
        public bool receiptPrinted { get; set; }
        public string receiptPrintedDate { get; set; }
        public string receiptPrintedUser { get; set; }

        /// <summary>
        /// Füllt die Felder aus
        /// Benötigt Kassenabschluss-Instanz und die zugehörigen Kassenabrechnung-Instanz und Kassenabschlussbeleg-Instanz
        /// </summary>
        /// <param name="cashClosure">Kassenabschluss-Instanz</param>
        /// <param name="cashClosureReport">Kassenabschlussbeleg-Instanz</param>
        /// <param name="cashClosureReceipt">Kassenabrechnung-Instanz</param>
        public CashClosureManagerDataGridModel(CashClosure cashClosure, CashClosureReport cashClosureReport, CashClosureReceipt cashClosureReceipt)
        {
            this.cashClosure = cashClosure;
            this.cashClosureReport = cashClosureReport;
            this.cashClosureReceipt = cashClosureReceipt;

            this.cashClosureID = cashClosure.CashClosureID;
            this.closureDate = SafeStringParser.safeParseToStr(cashClosure.ClosureDate, true);
            this.comment = SafeStringParser.safeParseToStr(cashClosure.Comment);
            this.revenues = SafeStringParser.safeParseToMoney(cashClosure.Revenue, true);
            this.expenses = SafeStringParser.safeParseToMoney(cashClosure.Expense, true);
            this.sum = SafeStringParser.safeParseToMoney(cashClosure.Sum, true);
            this.reportPrinted = cashClosureReport.PrintDone;
            this.reportPrintDate = SafeStringParser.safeParseToStr(cashClosureReport.PrintDate, true);
            this.reportDone = cashClosureReport.Done;
            this.reportDoneBoxEnabled = !(cashClosureReport.Done);
            this.reportDoneDate = SafeStringParser.safeParseToStr(cashClosureReport.DoneDate, true);
            this.receiptPrinted = cashClosureReceipt.PrintDone;
            this.receiptPrintedDate = SafeStringParser.safeParseToStr(cashClosureReceipt.PrintDate, true);

            if (cashClosure.ClosureUserAccount != null)
                this.closureUser = SafeStringParser.safeParseToStr(cashClosure.ClosureUserAccount.Username);
            else
                this.closureUser = "";

            if (cashClosureReport.PrintUserAccount != null)
                this.reportPrintUser = SafeStringParser.safeParseToStr(cashClosureReport.PrintUserAccount.Username);
            else
                this.reportPrintUser = "";

            if (cashClosureReport.DoneUserAccount != null)
                this.reportDoneUser = SafeStringParser.safeParseToStr(cashClosureReport.DoneUserAccount.Username);
            else
                this.reportDoneUser = "";

            if (cashClosureReceipt.PrintUserAccount != null)
                this.receiptPrintedUser = SafeStringParser.safeParseToStr(cashClosureReceipt.PrintUserAccount.Username);
            else
                this.receiptPrintedUser = "";

        }
    }
}
