using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is for Job
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using KC_Custom;

namespace JobExample
{
    public class TestJobObj
    {
        public Vector3 pos;
        public float movey;

        public TestJobObj()
        {
            pos = new Vector3(UnityEngine.Random.Range(-100, 100),UnityEngine.Random.Range(-100, 100),UnityEngine.Random.Range(-100, 100));
            movey = 1.0f;
        }
    }

    public class JobExample : MonoBehaviour
    {
        [SerializeField] private List<TestJobObj> m_testList = new List<TestJobObj>();
        [SerializeField] private int numberToHandler = 50;

        public enum EJOBTEST
        {
            J_PARALLEL,
            J_SINGLE,
            J_SINGLE_MULTIPLE,
            J_NOJOB,
        }

        [SerializeField] private EJOBTEST m_ejob = EJOBTEST.J_PARALLEL;

        void Start()
        {
            for(int i = 0; i < 100; i++)
            {
                TestJobObj t = new TestJobObj();
                this.m_testList.Add(t);
            }
        }

        private void Update() 
        {
            switch(this.m_ejob)
            {
                case EJOBTEST.J_PARALLEL:
                    this.ExecuteOneJobInParallel();
                    break;
                case EJOBTEST.J_SINGLE_MULTIPLE:
                    this.ExecuteMultipleJob();
                    break;
                case EJOBTEST.J_SINGLE:
                    this.ExecuteOneJob();
                    break;
                default:
                    this.NormalTaskCounter();
                    break;
            }
        }

        private void ExecuteOneJobInParallel()
        {
            NativeArray<float3> positionArr = new NativeArray<float3>(this.m_testList.Count,Allocator.TempJob);
            NativeArray<float> moveArray = new NativeArray<float>(this.m_testList.Count,Allocator.TempJob);

            for(int i = 0; i < this.m_testList.Count; i++)
            {
                positionArr[i] = this.m_testList[i].pos;
                moveArray[i] = this.m_testList[i].movey;
            }

            AJobExample ajob = new AJobExample{
                deltaTime = Time.deltaTime,
                posArr = positionArr,
                moveArr = moveArray,
            };

            JobHandle jh = ajob.Schedule(this.m_testList.Count, this.numberToHandler);
            jh.Complete();

            for(int i = 0; i < this.m_testList.Count; i++)
            {
                this.m_testList[i].pos = positionArr[i];
                this.m_testList[i].movey = moveArray[i];
            }
            
            positionArr.Dispose();
            moveArray.Dispose();
        }

        private void ExecuteMultipleJob()
        {
            NativeList<JobHandle> jhlist = new NativeList<JobHandle>(Allocator.Temp);
            for(int i = 0; i < 10; i++)
            {
                JobHandle jh = this.DoJobB();
                jhlist.Add(jh);
            }
            JobHandle.CompleteAll(jhlist);
            jhlist.Dispose(); //Have to dispose because is not collectable by c#
        }

        private void ExecuteOneJob()
        {
            JobHandle jobhandler = this.DoJobB();
            jobhandler.Complete();
        }
        
        
        private JobHandle DoJobB()
        {
            BJobExample exp = new BJobExample();
            return exp.Schedule();
        }

        private void NormalTaskCounter()
        {
            float value = 0.0f;
            for(int i = 0; i < 50000; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    
    }

    [BurstCompile]
    public struct AJobExample : IJobParallelFor
    {
        public NativeArray<float3> posArr;
        public NativeArray<float> moveArr;
        [ReadOnly] public float deltaTime;

        public void Execute(int index)
        {
            posArr[index] += new float3(0.0f, moveArr[index] * deltaTime, 0.0f);
            if(posArr[index].y > 100.0f)
            {
                moveArr[index] -= math.abs(moveArr[index]);
            }
            if(posArr[index].y < -100.0f)
            {
                moveArr[index] += math.abs(moveArr[index]);
            }


            float value = 0.0f;
            for(int i = 0; i < 50000; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    }

    [BurstCompile]
    public struct BJobExample : IJob
    {
        public void Execute()
        {
            float value = 0.0f;
            for(int i = 0; i < 50000; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    }
}