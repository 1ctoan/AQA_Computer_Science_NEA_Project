namespace NEA_NBody_3D_2D_GUI_Protoype
{
    internal class ReverseSimCircularStack
    {
        private SimulationSceneSnapshot[] circularStack;
        private int top;
        private int count;
        private int capacity;

        public ReverseSimCircularStack(int capacity)
        {
            this.capacity = capacity;
            circularStack = new SimulationSceneSnapshot[capacity];

            top = -1;
            count = 0;
        }

        public bool CheckStackIsEmpty()
        {
            return count == 0;
        }

        public bool CheckStackIsFull()
        {
            return count == capacity;
        }

        public void Push(SimulationSceneSnapshot bodyData)
        {
            top = (top + 1) % capacity;

            if (!CheckStackIsFull())
            {
                count++;
            }

            circularStack[top] = bodyData;
        }

        public SimulationSceneSnapshot Pop()
        {
            if (CheckStackIsEmpty())
            {
                throw new InvalidOperationException("Nothing left to rewind - only the last 3 minutes that was simulated is visible");
            }
            SimulationSceneSnapshot bodyData = circularStack[top];
            top = (top - 1 + capacity) % capacity;
            count--;

            return bodyData;
        }

        public SimulationSceneSnapshot Peek()
        {
            if (CheckStackIsEmpty())
            {
                throw new InvalidOperationException("Stack is empty");
            }

            return circularStack[top];
        }

    }

}
