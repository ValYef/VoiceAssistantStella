namespace VoiceAssistant.Components.Sphere
{
    public interface ISphereAnimator
    {
        void Initialize(int count);
        void Animate();
        void UpdateScaleAndSpeed(double volume);
    }
}
