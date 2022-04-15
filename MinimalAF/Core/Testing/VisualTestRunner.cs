using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MinimalAF {

    class DebugPanel : Element {
        public override void OnRender() {
            SetFont("Consolas", 24);
            SetDrawColor(Color4.VA(0, 1));

            Text("No more tests. Press esc to close", VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                GetAncestor<ApplicationWindow>().Close();
            }
        }
    }

    class NoMoreTests : Element {
        public override void OnMount(Window w) {
            SetClearColor(Color4.VA(1, 1));
        }

        public override void OnRender() {
            SetFont("Consolas", 24);

            Text("No more tests. Press esc to close", VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                GetAncestor<ApplicationWindow>().Close();
            }
        }
    }


    public class VisualTestRunner : Window {
        List<Type> visualTestElements = new List<Type>();
        ApplicationWindow window;
        Element currentTestElement;
        (int, int) wantedSize;

        ApplicationWindow debugWindow;
        Thread debugWindowThread;

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
            MinimalAFEnvironment.Debug = true;

            window = Parent.GetAncestor<ApplicationWindow>();

            debugWindowThread = new Thread(() => {
                debugWindow = new ApplicationWindow();
                debugWindow.Context.MakeCurrent();
                debugWindow.Run(new DebugPanel());
            });

            debugWindowThread.Start();

            FindAllVisualTests();

            Console.WriteLine("Found " + visualTestElements.Count + " tests:");
            foreach (Type t in visualTestElements) {
                Console.WriteLine("\t" + t.Name);
            }

            Console.WriteLine("Press escape to go to the next test");

            NextTest();
        }

        public override void AfterUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                if (!ranOutOfTests) {
                    NextTest();
                }
            }
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(0, 1));

            RectOutline(1, 0, 0, VW(1), VH(1));

            if (!ranOutOfTests)
                return;


        }

        int currentTest = 0;
        bool ranOutOfTests = false;

        public override string Title {
            get => window.Title;
            set => window.Title = value;
        }

        public override double UpdateFrequency {
            get => window.UpdateFrequency;
            set => window.UpdateFrequency = value;
        }

        public override double RenderFrequency {
            get => window.RenderFrequency;
            set => window.RenderFrequency = value;
        }

        public override WindowState WindowState {
            get;
            set;
        }

        public override (int, int) Size {
            get => ((int)RelativeRect.Width, (int)RelativeRect.Height);
            set {
                wantedSize = value;
                Layout();
            }
        }

        public override void OnLayout() {
            var newRect = currentTestElement.RelativeRect;
            newRect.SetWidth(wantedSize.Item1, 0.5f);
            newRect.SetHeight(wantedSize.Item2, 0.5f);
            currentTestElement.RelativeRect = newRect;

            LayoutChildren();
        }

        void NextTest() {
            RemoveChild(currentTestElement);
            currentTestElement = null;

            if (currentTest >= visualTestElements.Count) {
                ranOutOfTests = true;
                currentTestElement = new NoMoreTests();
                AddChild(currentTestElement);
                return;
            }

            Type t = visualTestElements[currentTest];
            currentTest++;
            try {
                Element test = (Element)Activator.CreateInstance(t);
                currentTestElement = test;

                AddChild(test);
            } catch (MissingMethodException mme) {
                throw new Exception("The class " + t.Name + " needs to have a parameterless constructor" +
                    "( e.g. var x = new " + t.Name + "() ) in order to be instantiated by this test harness.");
            } catch (Exception e) {
                throw e;
            }
        }

        public override void OnDismount() {
            Println("Joining Debug window thread...");
            debugWindow.Close();

            Println("Joining Debug window thread...");
            debugWindowThread.Join();
        }
    }
}
