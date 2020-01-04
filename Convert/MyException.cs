using System;
using System.Runtime.Serialization;

[Serializable()] //クラスがシリアル化可能であることを示す属性
public class MyException : Exception
{
    public MyException()
        : base()
    {
    }

    public MyException(string message)
        : base(message)
    {
    }

    public MyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    //逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
    protected MyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}