using System.Collections.Generic;
using CardZones;

public class EvaluationContext
{
    public int Value;

    public List<EvaluationStep> Steps = new();
    public DiscardPile Discard;
    
    public void AddStep(int newValue, string description, StepType type)
    {
        Steps.Add(new EvaluationStep
        {
            PreviousValue = Value,
            NewValue = newValue,
            Description = description,
            Type = type
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