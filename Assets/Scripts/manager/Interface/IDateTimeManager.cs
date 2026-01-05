using System;
using Senspark;

namespace manager.Interface
{
    [Service(nameof(IDateTimeManager))]
    public interface IDateTimeManager : IService
    {
        /// <summary>
        /// Đếm thời gian từ thời điểm hiện tại.
        /// </summary>
        void SetCountdownFromNow(string id, TimeSpan timeSpan);

        /// <summary>
        /// Thêm thời gian vào bộ đếm đã có.
        /// Nếu không tồn tại, sẽ thêm thời gian từ thời điểm hiện tại.
        /// </summary>
        void AppendCountdown(string id, TimeSpan timeSpan);

        /// <summary>
        /// Lấy thời gian kết thúc.
        /// </summary>
        /// <param name="id"></param>
        TimeSpan GetCountDownTimeLeft(string id);

        /// <summary>
        /// Kiểm tra đã hết giờ chưa.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsTimeOut(string id);
        /// <summary>
        /// kiểm tra đã có id trong bộ đếm chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsExist(string id);

        void Delete(string id);
        void OnNewDayRegister(Action action);
        void CheckNewDay();
    }
}