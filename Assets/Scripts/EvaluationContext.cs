using System.Collections.Generic;

public class EvaluationContext
{
    public int Value;

    public List<EvaluationStep> Steps = new();

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
}