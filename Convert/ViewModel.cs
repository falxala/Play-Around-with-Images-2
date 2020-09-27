using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reactive.Bindings;

namespace PlayAroundwithImages2
{

    public class ViewModel
    {
        public ReactiveProperty<int> Rotate { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<double> Scale_X { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Scale_Y { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> X { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Y { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Center_X { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Center_Y { get; } = new ReactiveProperty<double>();
        public ReactiveProperty<System.Windows.Point> RenderTransformOrigin_ { get; } = new ReactiveProperty<System.Windows.Point>();
    }
}
