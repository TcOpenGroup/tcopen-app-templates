using System.Collections.Generic;
using System.Linq;
using TcOpen.Inxton.Instructor;
using Vortex.Connector;
using Vortex.Connector.ValueTypes;
using x_template_xPlc;

namespace x_template_xInstructor.TcoSequencer
{
    public class InstructableSequencer : IInstructionControlProvider
    {
        private List<InstructionItem> _instructionSteps;

        public InstructableSequencer(TcoTaskedSequencer sequencer)
        {
            Sequencer = sequencer;
            Sequencer._currentStep.ID.Subscribe((sender, args) => this.ChangeStep(sender, args));
        }

        private TcoTaskedSequencer Sequencer { get; }

        public IEnumerable<InstructionItem> InstructionSteps
        {
            get { return this._instructionSteps; }
        }

        public string ProviderId => this.Sequencer.Symbol;

        public ChangeInstructionDelegate ChangeInstruction { get; set; }

        private void ChangeStep(IValueTag sender, ValueChangedEventArgs args)
        {
            ChangeInstruction?.Invoke(args.NewValue.ToString());
        }

        public void UpdateTemplate()
        {
            _instructionSteps = new List<InstructionItem>();
            this.Sequencer.Read();

            foreach (var step in Sequencer._o._steps.Where(x=>x.ID.Cyclic != 0))
            {
                _instructionSteps.Add(new InstructionItem()
                {
                    Key = step.ID.LastValue.ToString(),
                    Remarks = step.Description.LastValue
                });
            }
        }
    }
}
