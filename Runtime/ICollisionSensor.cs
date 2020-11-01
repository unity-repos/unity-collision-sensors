using System;

namespace CollisionSensors.Runtime
{
    public interface ICollisionSensor
    {
        Action CallbackCollisionEnter { get; set; }
        Action CallbackCollisionExit { get; set; }
        int Count { get; }
    }
}