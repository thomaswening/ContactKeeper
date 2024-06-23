using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactKeeper.Core.Exceptions;


public class RepositoryCorruptedException(string message, Exception? innerException = null) : Exception(message, innerException)
{
}
