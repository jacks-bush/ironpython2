using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;

namespace ReproduceError {
    class Program {
        static void Main(string[] args) {
            string mainCode = @"
import sys

def test(frame, event, args):
    pass

sys.settrace(test)

class BasicClass:
    pass

BasicClass()
";

            ScriptEngine engine = Python.CreateEngine(new Dictionary<string, object>() { { "CompilationThreshold", 0 } });
            ScriptScope scope = engine.CreateScope();
            engine.Execute(mainCode, scope);
        }
    }
}
