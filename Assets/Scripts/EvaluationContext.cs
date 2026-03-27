using System.Collections.Generic;
using CardZones;
using Stickers;

public class EvaluationContext
{
    public int Value;

    public List<EvaluationStep> Steps = new();
    public DiscardPile Discard;
    
    public void AddStep(int newValue, string description, StepType type, ISticker source = null)
    {
        Steps.Add(new EvaluationStep
        {
            PreviousValue = Value,
            NewValue = newValue,
            Description = description,
            Type = type,
            Source = source
        });

        Value = newValue;
    }
    
    public EvaluationContext CloneBase() =>
        new()
        {
            Value = Value,
            Discard = Discard
        };
}