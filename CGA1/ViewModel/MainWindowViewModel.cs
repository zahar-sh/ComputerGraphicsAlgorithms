﻿using CGA1.Command;
using CGA1.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System;

namespace CGA1.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private float _modelPosX;
        private float _modelPosY;
        private float _modelPosZ;

        private float _modelYaw;
        private float _modelPitch;
        private float _modelRoll;

        private float _modelScale;

        private float _cameraPosX;
        private float _cameraPosY;
        private float _cameraPosZ;

        private float _cameraYaw;
        private float _cameraPitch;
        private float _cameraRoll;

        private Color _color;

        private IObjPainter _objPainter;

        private Obj _obj;
        private Image _image;

        public MainWindowViewModel()
        {
            FileDialog = new OpenFileDialog
            {
                Filter = "Assembly | *.dll",
                Title = "Select assembly",
                Multiselect = false
            };
            LoadObjCommand = new DelegateCommand(o => LoadObj());
            RepaintCommand= new DelegateCommand(o => Repaint());
        }

        public float ModelPosX { get => _modelPosX; set => SetProperty(ref _modelPosX, value); }
        public float ModelPosY { get => _modelPosY; set => SetProperty(ref _modelPosY, value); }
        public float ModelPosZ { get => _modelPosZ; set => SetProperty(ref _modelPosZ, value); }

        public float ModelYaw { get => _modelYaw; set => SetProperty(ref _modelYaw, value); }
        public float ModelPitch { get => _modelPitch; set => SetProperty(ref _modelPitch, value); }
        public float ModelRoll { get => _modelRoll; set => SetProperty(ref _modelRoll, value); }

        public float ModelScale { get => _modelScale; set => SetProperty(ref _modelScale, value); }

        public float CameraPosX { get => _cameraPosX; set => SetProperty(ref _cameraPosX, value); }
        public float CameraPosY { get => _cameraPosY; set => SetProperty(ref _cameraPosY, value); }
        public float CameraPosZ { get => _cameraPosZ; set => SetProperty(ref _cameraPosZ, value); }

        public float CameraYaw { get => _cameraYaw; set => SetProperty(ref _cameraYaw, value); }
        public float CameraPitch { get => _cameraPitch; set => SetProperty(ref _cameraPitch, value); }
        public float CameraRoll { get => _cameraRoll; set => SetProperty(ref _cameraRoll, value); }

        public Color Color { get => _color; set => SetProperty(ref _color, value); }

        public IObjPainter ObjPainter { get => _objPainter; set => SetProperty(ref _objPainter, value); }

        public Obj Obj { get => _obj; set => SetProperty(ref _obj, value); }

        public Image Image { get => _image; set => SetProperty(ref _image, value); }

        public ICommand LoadObjCommand { get; }

        public ICommand RepaintCommand { get; }

        private FileDialog FileDialog { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadObj()
        {
            var open = FileDialog.ShowDialog();
            if (open != null && open.Value)
            {
                var fileName = FileDialog.FileName;
                using (var reader = new StreamReader(new FileStream(fileName, FileMode.Open)))
                {
                    Obj = ObjParser.Parse(reader);
                }
            }
        }

        private void Repaint()
        {
            var width = (int)Image.ActualWidth;
            var height = (int)Image.ActualHeight;
            var image = new WritableImage(width, height);

            var viewportMatrix = Matrices.CreateViewportMatrix(0, 0, width, height);
            var projectionMatrix = Matrices.CreateProjectionByAspect((float)width / height, ToRadians(60), 0.1f, 100.0f);
            var viewMatrix = Matrices.CreateViewMatrix(CameraPosX, CameraPosY, CameraPosZ, CameraYaw, CameraPitch, CameraRoll);
            var modelMatrix = Matrices.CreateModelMatrix(ModelPosX, ModelPosY, ModelPosZ, ModelYaw, ModelPitch, ModelRoll, ModelScale);

            var model = Obj.Transform(viewportMatrix, projectionMatrix, viewMatrix, modelMatrix);

            ObjPainter.Paint(model, image, Color);

            Image.Source = image.Source;

            NotifyPropertyChanged(nameof(Image));
        }

        private static float ToRadians(float radians)
        {
            return (float)(radians * Math.PI / 180);
        }
    }
}
