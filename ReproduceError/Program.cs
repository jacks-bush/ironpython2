using System.Threading;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using System.IO;

namespace ReproduceError {
    class Program {

        static void Main(string[] args) {

            // perhaps try also adding a console?
            string mainCode = @"
pyclass.ExecuteWithoutTracing()

if True:
    if True:
        if True:
            if True:
                basicClass = getBasicClass()
                print basicClass.Hello
";

            // doesn't fail every time so run in a loop until it does
            while (true) {
                // create engine and scope
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();

                // set initial tracing
                engine.SetTrace(Debugger.OnTracebackReceived);

                // set variable in engine
                scope.SetVariable("pyclass", new PyClass(engine, scope));

                // execute main code
                engine.Execute(mainCode, scope);
            }
        }
    }

    public class Debugger {
        public static TracebackDelegate OnTracebackReceived(TraceBackFrame frame, string result, object payload) {
            return OnTracebackReceived;
        }
    }

    public class PyClass {

        private ScriptEngine _engine;
        private ScriptScope _scope;

        private const string code = @"
def getBasicClass():
	
    class BasicClass:
        pass
    basic = BasicClass()
    basic.Hello = ""Hello, cruel world""

    return basic
";

        public PyClass(ScriptEngine engine, ScriptScope scope) {
            _engine = engine;
            _scope = scope;
        }

        public void ExecuteWithoutTracing() {
            _engine.SetTrace(null);
            _engine.Execute(code, _scope);
            _engine.SetTrace(Debugger.OnTracebackReceived);
        }
    }
}
