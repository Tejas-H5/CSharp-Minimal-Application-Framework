using MinimalAF;
using MinimalAF.Audio;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngineTests.AudioTests {
    public class ComplicatedWavPLayingTest : IRenderable {
        AudioClipInput _clackSound;
        AudioListener _listener;
        DrawableFont _font = new DrawableFont("Consolas", 16);
        Random _random = new Random();

        public ComplicatedWavPLayingTest() {
            _clackSound = new AudioClipInput(
                AudioRawData.FromFile("./Res/keyboardClack0.wav")
            );

            _listener = new AudioListener();
        }

        void DrawCrosshairs(ref AFContext ctx, float x, float y, float size) {
            IM.DrawLine(ctx, x - size - 50, y, x - size + 50, y, 10, CapType.Circle);
            IM.DrawLine(ctx, x + size - 50, y, x + size + 50, y, 10, CapType.Circle);
            IM.DrawLine(ctx, x, y - size - 50, x, y - size + 50, 10, CapType.Circle);
            IM.DrawLine(ctx, x, y + size - 50, x, y + size + 50, 10, CapType.Circle);

            IM.DrawCircleOutline(ctx, 10, x, y, size);
        }

        float _barrelSpeed = 0; // between 0 and 1
        float _bulletsToFire = 0;
        public float BarrelMaxFrequency = 1000;
        public float BarrelResponse = 2f;

        struct Shot {
            public float X;
            public float Y;
            public AudioSource AudioSource;
        }
        List<Shot> _shots = new List<Shot>();


        public void Render(AFContext ctx) {
            float aimPosX = ctx.MouseX;
            float aimPosY = ctx.MouseY;
            float spreadRadius = 100;

            ctx.SetDrawColor(Color.White);
            IM.DrawRect(ctx, 0, 0, ctx.VW, ctx.VH);

            bool isFiring = ctx.MouseButtonIsDown(MouseButton.Left);

            // draw crosshairs
            {
                if (isFiring) {
                    ctx.SetDrawColor(Color.Red, 1.0f);
                } else {
                    ctx.SetDrawColor(Color.Red, 0.5f);
                }
                DrawCrosshairs(ref ctx, aimPosX, aimPosY, spreadRadius);
                if (isFiring) {
                    _font.DrawText(ctx, ">>> Firing", aimPosX + spreadRadius, aimPosY - spreadRadius);
                } else {
                    _font.DrawText(ctx, "> Ready", aimPosX + spreadRadius, aimPosY - spreadRadius);
                }
            }

            // draw shots
            {
                ctx.SetDrawColor(Color.Black);
                for (int i = 0; i < _shots.Count; i++) {
                    IM.DrawCircle(ctx, _shots[i].X, _shots[i].Y, 10.0f);

                    if (_shots[i].AudioSource.PlaybackState == PlaybackState.Stopped) {
                        // programmers hate this one simple trick
                        _shots.RemoveAt(i); i--;
                    }
                }
            }

            // draw minigun progress bar
            {
                ctx.SetDrawColor(
                    Color.Lerp(Color.Red, Color.GreenLime, _barrelSpeed)
                );
                IM.DrawRect(ctx, 0, ctx.VH - 5, ctx.VW * _barrelSpeed, ctx.VH);
            }

            // draw shots

            // update minigun barrel
            {

                _barrelSpeed = MathHelpers.MoveTowards(
                    _barrelSpeed,
                    isFiring ? 1 : 0,
                    BarrelResponse * Time.DeltaTime
                );

                _bulletsToFire += (_barrelSpeed * BarrelMaxFrequency) * Time.DeltaTime;

                for(;_bulletsToFire > 1.0f; _bulletsToFire -= 1.0f) {
                //if (_bulletsToFire > 1.0f) {
                //    _bulletsToFire -= 1.0f;

                    // fire a bullet

                    var shot = new Shot {
                        X = aimPosX + (0.5f - _random.NextSingle()) * spreadRadius,
                        Y = aimPosY + (0.5f - _random.NextSingle()) * spreadRadius,
                        AudioSource = new AudioSource()
                    };
                    _shots.Add(shot);

                    // shot.AudioSource.Position = new Vector3(shot.X, 0, shot.Y);
                    shot.AudioSource.Position = new Vector3(0, 0, 0);
                    shot.AudioSource.SetInput(_clackSound);
                    shot.AudioSource.Play();    // our minigun shoots mechanical keyboard switches? who wrote this test?
                }
            }

            // update listener 
            {
                // _listener.Position = new Vector3(ctx.VW / 2, 0, ctx.VH / 2);
                _listener.MakeCurrent();
                _listener.Position = (0, 0, 0);
            }
        }
    }
}
