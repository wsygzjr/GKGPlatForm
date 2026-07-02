using GKG.ElectronicControl;
using GKG.MotionControl;
using Griffins.PF.Server;

namespace GKG.SubMM
{
    internal static class ElectronicFunc
    {
        public static List<IBaseStateIO> GetStateIOInstancesByIds(List<Guid> ioGuids)
        {
            if (ioGuids == null || ioGuids.Count == 0)
                return new List<IBaseStateIO>();

            var response = ServerInnerInfoSender.SendMutualInfo(
                StateIOInstancesByIdsRequest.InfoKindID,
                new StateIOInstancesByIdsRequest(ioGuids));

            if (response == null || response.Count == 0)
                throw new InvalidOperationException("获取气缸 IO 实例失败。");

            StateIOInstancesByIdsResponse? ioResponse = response[0].Response as StateIOInstancesByIdsResponse;
            List<IBaseStateIO> ioInstances = ioResponse?.StateIOInstances ?? new List<IBaseStateIO>();
            if (ioInstances.Count != ioGuids.Count)
                throw new InvalidOperationException($"获取到的气缸 IO 实例数量异常，期望 {ioGuids.Count}，实际 {ioInstances.Count}。");

            return ioInstances;
        }
        public static IRobotDriver CreateRobotDriver(Guid axisBindingObjID, string axisMissingMsg, string responseMissingMsg, string unavailableMsg)
        {
            if (axisBindingObjID == Guid.Empty)
                throw new InvalidOperationException(axisMissingMsg);

            RobotDriverByAxisIdsRequest request = new RobotDriverByAxisIdsRequest(new List<Guid> { axisBindingObjID })
            {
                MotionCardType = MotionControlCardType.Normal
            };

            var robotDriverResponses = ServerInnerInfoSender.SendMutualInfo(RobotDriverByAxisIdsRequest.InfoKindID, request);
            if (robotDriverResponses == null || robotDriverResponses.Count == 0)
                throw new InvalidOperationException(responseMissingMsg);

            RobotDriverByAxisIdsResponse robotDriverResponse = robotDriverResponses[0].Response as RobotDriverByAxisIdsResponse;
            if (robotDriverResponse?.RobotDriver == null)
                throw new InvalidOperationException(unavailableMsg);

            return robotDriverResponse.RobotDriver;
        }
    }
}
