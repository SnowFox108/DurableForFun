using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PdfGenerator.Infrastructure;

public enum ContractNoteScenario
{
    [EnumMember(Value = "Migration")]
    Migration = 0,
    [EnumMember(Value = "Indicative")]
    Indicative = 1,
    [EnumMember(Value = "Creation")]
    Creation = 2,
    [EnumMember(Value = "Expiry")]
    Expiry = 3,

    //Used in UI
    [EnumMember(Value = "Bank Amount Update")]
    BankAmountUpdate = 4,


    //Used in UI
    [EnumMember(Value = "Close out")]
    Closeout = 5,

    [EnumMember(Value = "Termination")]
    Termination = 6,
    //used 
    [EnumMember(Value = "Valuation Report")]
    ValuationReport = 7

}