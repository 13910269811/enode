using System;
using System.Collections;

namespace ENode.Infrastructure.Concurrent
{
    /// <summary>�ṩ����ֵ��������Ϊ��ʵ�÷���
    /// </summary>
    public static class LockUtility
    {
        private class LockObject
        {
            public int Counter { get; set; }
        }

        /// <summary>�������, �������ü�������0�������󶼻��ڳ��л�������
        /// </summary>
        private static readonly Hashtable LockPool = new Hashtable();

        /// <summary>�÷������Ը���ĳ��ֵ�����ֵ����ס��Ӧ����Ϊ��
        /// �÷�����ϵͳlock��������������סĳ��ָ����key��ֻҪkey��ֵ��ͬ����ôaction��ִ�оͲ���������
        /// <remarks>
        /// ��Ƹ÷�����Ϊ���ֲ�.net��ܵ�lock�����ľ����ԡ�.net��ܵ�lock����ֻ����������ͬ�Ķ���
        /// ���Ǻܶ�ʱ������ϣ����סĳ��ֵ����ֻҪ��ֵ�����ֵ��ͬ����action�Ͳ���������
        /// </remarks>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void Lock(object key, Action action)
        {
            var lockObj = GetLockObject(key);
            try
            {
                lock (lockObj)
                {
                    action();
                }
            }
            finally
            {
                ReleaseLockObject(key, lockObj);
            }
        }

        /// <summary>�ͷ�������, ���������ü���Ϊ0ʱ, ����������Ƴ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockObj"></param>
        private static void ReleaseLockObject(object key, LockObject lockObj)
        {
            lockObj.Counter--;
            lock (LockPool)
            {
                if (lockObj.Counter == 0)
                {
                    LockPool.Remove(key);
                }
            }
        }
        /// <summary>����������л�ȡ������, ��������������ü�����1.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static LockObject GetLockObject(object key)
        {
            lock (LockPool)
            {
                var lockObj = LockPool[key] as LockObject;
                if (lockObj == null)
                {
                    lockObj = new LockObject();
                    LockPool[key] = lockObj;
                }
                lockObj.Counter++;
                return lockObj;
            }
        }
    }
}