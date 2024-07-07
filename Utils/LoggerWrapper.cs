using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GreatUma.Utils
{
    public static class LoggerWrapper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static string GetExceptionMessage(Exception ex)
        {
            return $"Message:[{ex.Message}]\nStackTrace:[{ex.StackTrace}]";
        }

        private static string GetMessage(string str, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return $"[{Path.GetFileName(sourceFilePath)}:{sourceLineNumber},{memberName}] {str}";
        }

        public static void Debug(Exception ex,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Debug(GetExceptionMessage(ex), memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Info(Exception ex,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Info(GetExceptionMessage(ex), memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Warn(Exception ex,
           [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
           [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
           [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Warn(GetExceptionMessage(ex), memberName, sourceFilePath, sourceLineNumber);
        }
        public static void Error(Exception ex,
           [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
           [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
           [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(GetExceptionMessage(ex), memberName, sourceFilePath, sourceLineNumber);
        }


        public static void Debug(string str,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Debug(GetMessage(str, memberName, sourceFilePath, sourceLineNumber));
        }

        public static void Info(string str,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Info(GetMessage(str, memberName, sourceFilePath, sourceLineNumber));
        }

        public static void Warn(string str,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Warn(GetMessage(str, memberName, sourceFilePath, sourceLineNumber));
        }

        public static void Error(string str,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            _logger.Error(GetMessage(str, memberName, sourceFilePath, sourceLineNumber));
        }


    }
}

