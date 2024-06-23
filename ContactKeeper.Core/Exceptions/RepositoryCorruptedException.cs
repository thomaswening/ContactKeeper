using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.Core.Interfaces;

namespace ContactKeeper.Core.Exceptions;


public class RepositoryCorruptedException(string? message = null, Exception? innerException = null) : Exception(message, innerException)
{
    
}
