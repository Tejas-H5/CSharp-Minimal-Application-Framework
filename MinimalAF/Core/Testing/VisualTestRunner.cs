using System;
using System.Collections.Generic;
using System.Reflection;

namespace MinimalAF {
    public class VisualTestRunner : Element {
        List<Type> visualTestElements = new List<Type>();

        public void FindAllVisualTests() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in assembly.GetTypes()) {
                    if (t.GetCustomAttribute(typeof(VisualTestAttribute)) == null) {
                        continue;
                    }

                    if (t.BaseType != typeof(Element)) {
                        throw new Exception("Class " + t.Name + " does not inherit from the " + typeof(Element).Name + " class. You may have put the [VisualTestAttribute] on it by accident");
                    }

                    visualTestElements.Add(t);
                }
            }
        }

        public override void OnMount(Window w) {
            FindAllVisualTests();

            Console.WriteLine("Found " + visualTestElements.Count + " tests:");
            foreach (Type t in visualTestElements) {
                Console.WriteLine("\t" + t.Name);
            }

            Console.WriteLine("Press escape to go to the next test");

            NextTest();
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                if (!ranOutOfTests) {
                    NextTest();
                } else {
                    GetAncestor<Window>().Close();
                }
            }
        }

        public override void OnRender() {
            if (!ranOutOfTests)
                return;

            SetDrawColor(Color4.VA(0, 1));
            SetFont("Consolas", 24);

            Text("No more tests. Press esc to close", VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        int currentTest = 0;
        bool ranOutOfTests = false;

        void NextTest() {
            if (currentTest >= visualTestElements.Count) {
                SetChildren(null);
                ranOutOfTests = true;
                SetClearColor(Color4.VA(1, 1));
                return;
            }

            Type t = visualTestElements[currentTest];
            currentTest++;
            try {
                Element test = (Element)Activator.CreateInstance(t);

                SetChildren(test);
            } catch(MissingMethodException mme) {
                throw new Exception("The class " + t.Name + " needs to have a parameterless constructor" +
                    "( e.g. var x = new " + t.Name + "() ) in order to be instantiated by this test harness.");
            }
        }
    }
}
