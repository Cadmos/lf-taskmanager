using System.Threading.Tasks;
using LF.ManagerCore.Runtime;
using LF.TaskManager.Runtime;
using UnityEngine;

namespace LF.TaskManager.Samples
{
    public class Samples1 : MonoBehaviour
    {
        public enum TaskState { NotRunning, Running, Completed }
        public TaskState task1State = TaskState.NotRunning;
        public TaskState task2State = TaskState.NotRunning;
        public TaskState taskPriorityState = TaskState.NotRunning;

        public async Task Task1()
        {
            task1State = TaskState.Running;
            await ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                await Task.Delay(1000);
                Debug.Log("Task 1 completed after 1 second.");
                task1State = TaskState.Completed;
            }, TaskQueuePriority.Normal);
        }

        public async Task Task2()
        {
            task2State = TaskState.Running;
            await ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                await Task.Delay(2000);
                Debug.Log("Task 2 completed after 2 seconds.");
                task2State = TaskState.Completed;
            }, TaskQueuePriority.High);
        }

        public async Task TaskWithPriority(TaskQueuePriority priority, int delayInMilliseconds)
        {
            taskPriorityState = TaskState.Running;
            await ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                await Task.Delay(delayInMilliseconds);
                Debug.Log($"Task with {priority} priority completed after {delayInMilliseconds} ms.");
                taskPriorityState = TaskState.Completed;
            }, priority);
        }
    }
}