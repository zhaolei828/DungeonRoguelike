using System.Collections.Generic;

/// <summary>
/// 关卡生成器接口
/// 定义地牢生成算法的标准接口
/// </summary>
public interface ILevelBuilder
{
    /// <summary>
    /// 生成房间布局
    /// </summary>
    /// <param name="level">要生成的关卡</param>
    /// <returns>生成的房间列表</returns>
    List<Room> Build(Level level);
}

