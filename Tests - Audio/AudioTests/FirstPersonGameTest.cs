using MinimalAF;
using MinimalAF.Audio;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // TODO: add audio.
    // Is this thing big enough to be an example?
    // I think I would only make it an example when I start actually trying to make a maintainable
    // software and not just some demo thing
    public class FirstPersonGameTest : IRenderable, IDisposable {
        struct GameObject {
            public Vector3 Position;
            public Quaternion Rotation;
            public AudioSource Source;

            public Matrix4 Transform => Matrix4.CreateTranslation(Position) * Matrix4.CreateFromQuaternion(Rotation);
        }

        AudioListener _listener;
        DrawableFont _font = new DrawableFont("Consolas", 16);
        GameObject[] _gameObjects;
        GameObject _player;

        Vector3 _position = new Vector3(0, 0, -10);
        float _hAngleCurrent, _vAngleCurrent;
        float _hAngleWanted, _vAngleWanted;
        bool _paused;

        public FirstPersonGameTest() {
            _listener = new AudioListener();
            _listener = new AudioListener();

            var audioData = AudioRawData.FromFile("./Res/testMusicShort.mp3");

            // make the scene
            var gameObjects = new List<GameObject>();
            {
                _player = new GameObject {
                    Position = new Vector3(0, 0, 0),
                    Rotation = Quaternion.Identity,
                    Source = new AudioSource()
                };

                var stream = new AudioStreamRawData(audioData);
                stream.PlaybackEndBehaviour = PlaybackEndBehaviourType.Loop;
                _player.Source.SetInput(
                    new AudioStreamInput(stream)
                );
                _player.Source.Play();
                gameObjects.Add(_player);
            }

            // OMG is this an ECS SYSTEM ?? NOW AY GUYS !!!? ?!
            _gameObjects = gameObjects.ToArray();
        }

        void RenderCube(ref AFContext ctx) {
            var corner1 = new Vector3(-0.5f, -0.5f, -0.5f);
            var corner2 = new Vector3(0.5f, 0.5f, 0.5f);
            var vx1y1z1 = new Vertex(new Vector3(corner1.X, corner1.Y, corner1.Z), new Vector2());
            var vx2y1z1 = new Vertex(new Vector3(corner2.X, corner1.Y, corner1.Z), new Vector2());
            var vx1y2z1 = new Vertex(new Vector3(corner1.X, corner2.Y, corner1.Z), new Vector2());
            var vx2y2z1 = new Vertex(new Vector3(corner2.X, corner2.Y, corner1.Z), new Vector2());
            var vx1y1z2 = new Vertex(new Vector3(corner1.X, corner1.Y, corner2.Z), new Vector2());
            var vx2y1z2 = new Vertex(new Vector3(corner2.X, corner1.Y, corner2.Z), new Vector2());
            var vx1y2z2 = new Vertex(new Vector3(corner1.X, corner2.Y, corner2.Z), new Vector2());
            var vx2y2z2 = new Vertex(new Vector3(corner2.X, corner2.Y, corner2.Z), new Vector2());

            ctx.SetDrawColor(Color.Blue);
            IM.DrawQuad(ctx, vx1y1z1, vx1y2z1, vx2y2z1, vx2y1z1);
            ctx.SetDrawColor(Color.Lerp(Color.Blue, Color.White, 0.5f));
            IM.DrawQuad(ctx, vx1y1z2, vx1y2z2, vx2y2z2, vx2y1z2);

            ctx.SetDrawColor(Color.Green);
            IM.DrawQuad(ctx, vx1y1z1, vx1y1z2, vx2y1z2, vx2y1z1);
            ctx.SetDrawColor(Color.Lerp(Color.Green, Color.White, 0.5f));
            IM.DrawQuad(ctx, vx1y2z1, vx1y2z2, vx2y2z2, vx2y2z1);

            ctx.SetDrawColor(Color.Red);
            IM.DrawQuad(ctx, vx1y1z1, vx1y1z2, vx1y2z2, vx1y2z1);
            ctx.SetDrawColor(Color.Lerp(Color.Red, Color.White, 0.5f));
            IM.DrawQuad(ctx, vx2y1z1, vx2y1z2, vx2y2z2, vx2y2z1);
        }


        Quaternion _rotation;

        void UpdateGame(ref AFContext ctx) {
            _hAngleWanted += ctx.MouseXDelta * 0.005f;
            _vAngleWanted -= ctx.MouseYDelta * 0.005f;
            _vAngleWanted = MathHelper.Clamp(_vAngleWanted, -MathF.PI / 2f + 0.1f, MathF.PI / 2f - 0.1f);

            // float lookSpeedRadiansPerSec = MathF.PI;
            float lookSpeedRadiansPerSec = 40;

            _hAngleCurrent = MathHelpers.Lerp(_hAngleCurrent, _hAngleWanted, lookSpeedRadiansPerSec * Time.DeltaTime);
            _vAngleCurrent = MathHelpers.Lerp(_vAngleCurrent, _vAngleWanted, lookSpeedRadiansPerSec * Time.DeltaTime);

            _rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), _hAngleCurrent) *
                Quaternion.FromAxisAngle(new Vector3(1, 0, 0), _vAngleCurrent);


            float speed = 5;
            if (ctx.KeyIsDown(KeyCode.W)) {
                _position += _rotation * new Vector3(0, 0, 1) * Time.DeltaTime * speed;
            }
            if (ctx.KeyIsDown(KeyCode.S)) {
                _position -= _rotation * new Vector3(0, 0, 1) * Time.DeltaTime * speed;
            }
            if (ctx.KeyIsDown(KeyCode.D)) {
                _position += _rotation * new Vector3(1, 0, 0) * Time.DeltaTime * speed;
            }
            if (ctx.KeyIsDown(KeyCode.A)) {
                _position -= _rotation * new Vector3(1, 0, 0) * Time.DeltaTime * speed;
            }
            if (ctx.KeyIsDown(KeyCode.E)) {
                _position += new Vector3(0, 1, 0) * Time.DeltaTime * speed;
            }
            if (ctx.KeyIsDown(KeyCode.Q)) {
                _position -= new Vector3(0, 1, 0) * Time.DeltaTime * speed;
            }
        }

        void RenderGame(ref AFContext ctx) {
            // Position camera/player
            {
                ctx.SetProjectionPerspective(MathF.PI * 0.5f, 0.01f, 1000);
                ctx.SetViewOrientation(_position, _rotation);

                _listener.MakeCurrent();
                _listener.Position = _position;
                _listener.Rotation = _rotation;
                // ctx.SetViewLookAt(_position, _position + rotation * new Vector3(0, 0, 1), new Vector3(0, 1, 0));
                // ctx.SetViewLookAt(
                //     _position,
                //     _position + _rotation * new Vector3(0, 0, 1),
                //     new Vector3(0, 1, 0)
                // );
            }

            // Draw things in the scene
            {
                for (int i = 0; i < _gameObjects.Length; i++) {
                    _gameObjects[i].Source.Position = _gameObjects[i].Position;

                    ctx.SetModel(_gameObjects[i].Transform);
                    RenderCube(ref ctx);
                }
            }


            // Draw ui 
            {
                ctx.Use();

                // Debug
                {
                    ctx.SetDrawColor(Color.Black);
                    _font.DrawText(
                        ctx, "Position: " + _position + "\n" + "Angles: " + _hAngleCurrent + " " + _vAngleCurrent,
                        0, ctx.VH,
                        HAlign.Left, VAlign.Top
                    );
                }


                if (_paused) {
                    ctx.SetDrawColor(Color.Black);
                    _font.DrawText(ctx, "Paused", ctx.VW / 2, ctx.VH / 2, HAlign.Center, VAlign.Center);
                    ctx.MouseState = OpenTK.Windowing.Common.CursorState.Normal;
                } else {
                    // Crosshairs
                    {
                        float crosshairSize = 20;
                        ctx.SetDrawColor(Color.Black);
                        IM.DrawLine(
                            ctx,
                            ctx.VW / 2 - crosshairSize, ctx.VH / 2,
                            ctx.VW / 2 + crosshairSize, ctx.VH / 2,
                            3
                        );

                        IM.DrawLine(
                            ctx,
                            ctx.VW / 2, ctx.VH / 2 - crosshairSize,
                            ctx.VW / 2, ctx.VH / 2 + crosshairSize,
                            3
                        );
                    }

                    ctx.MouseState = OpenTK.Windowing.Common.CursorState.Grabbed;
                }
            }
        }   

        public void Render(AFContext ctx) {
            ctx.SetClearColor(Color.White);

            // if we have started rendering after being disposed, we reactivate our things
            if (_disposed) {
                _disposed = false;

                // start all sounds we paused
                for (int i = 0; i < _gameObjects.Length; i++) {
                    if (_gameObjects[i].Source.PlaybackState == PlaybackState.Paused) {
                        _gameObjects[i].Source.Play();
                    }
                }
            }

            // process inputs
            {
                if (ctx.KeyJustPressed(KeyCode.Escape)) {
                    _paused = !_paused;
                }

                if (!_paused) {
                    UpdateGame(ref ctx);
                }
            }


            RenderGame(ref ctx);
        }

        bool _disposed = false;
        public void Dispose() {
            if (_gameObjects == null) return;
            if (_disposed) return;

            _disposed = true;

            // pause all sounds we are playing
            for (int i = 0; i < _gameObjects.Length; i++) {
                _gameObjects[i].Source.Pause();
            }
        }
    }
}
