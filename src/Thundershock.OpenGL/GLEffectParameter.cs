using System.Numerics;
using Thundershock.Core.Rendering;

using Silk.NET.OpenGL;

namespace Thundershock.OpenGL
{
    public class GLEffectParameter : EffectParameter
    {
        private GL _gl;
        private string _name;
        private int _location;
        private uint _program;

        public override string Name => _name;

        public override void SetValue(int value)
        {
            _gl.ProgramUniform1(_program, _location, value);
        }

        public override void SetValue(float value)
        {
            _gl.ProgramUniform1(_program, _location, value);
        }

        public override void SetValue(double value)
        {
            _gl.ProgramUniform1(_program, _location, value);
        }

        public override void SetValue(uint value)
        {
            _gl.ProgramUniform1(_program, _location, value);
        }

        public override void SetValue(Vector2 value)
        {
            _gl.ProgramUniform2(_program, _location, ref value);
        }

        public override void SetValue(Vector3 value)
        {
            _gl.ProgramUniform3(_program, _location, ref value);
        }

        public override void SetValue(Vector4 value)
        {
            _gl.ProgramUniform4(_program, _location, ref value);
        }

        public override void SetValue(Matrix4x4 value)
        {
            var data = new float[16];

            data[0] = value.M11;
            data[1] = value.M21;
            data[2] = value.M31;
            data[3] = value.M41;

            data[4] = value.M12;
            data[5] = value.M22;
            data[6] = value.M32;
            data[7] = value.M42;

            data[8] = value.M13;
            data[9] = value.M23;
            data[10] = value.M33;
            data[11] = value.M43;

            data[12] = value.M14;
            data[13] = value.M24;
            data[14] = value.M34;
            data[15] = value.M44;

            _gl.ProgramUniformMatrix4(_program, _location, 1, false, data);
        }

        public override byte GetValueByte()
        {
            throw new System.NotImplementedException();
        }

        public override int GetValueInt32()
        {
            throw new System.NotImplementedException();
        }

        public override uint GetValueUInt32()
        {
            throw new System.NotImplementedException();
        }

        public override float GetValueFloat()
        {
            throw new System.NotImplementedException();
        }

        public override double GetValueDouble()
        {
            throw new System.NotImplementedException();
        }

        public override Vector2 GetVector2()
        {
            throw new System.NotImplementedException();
        }

        public override Vector3 GetVector3()
        {
            throw new System.NotImplementedException();
        }

        public override Vector4 GetVector4()
        {
            throw new System.NotImplementedException();
        }

        public override Matrix4x4 GetMatric4x4()
        {
            throw new System.NotImplementedException();
        }

        public GLEffectParameter(uint program, string name, int location, GL gl)
        {
            _gl = gl;
            _program = program;
            _name = name;
            _location = location;
        }
    }
}