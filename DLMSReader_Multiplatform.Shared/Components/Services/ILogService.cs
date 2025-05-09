using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSReader_Multiplatform.Shared.Components.Services
{
    public interface ILogService
    {
        void Write(string message);
        IReadOnlyList<string> Lines { get; } //Kolekce vsech aktualnich radku... IReadOnlyList --> Jsou pouze pro cteni


        //Toto je udalost, na kterou prihlasujeme UI komponentu.
        //Udalost se zavola pri kazdem novem zapisu --> UI se to dozvi a zavola StatHasChanged a prerenderuje se resp dopise do konzole dalsi radek...
        event Action? LogUpdated; 
    }
}
