# Plain Craft Launcher (PCL) fix-network

此分支是基于 PCL 2.99.0.0 的第三方独立测试构建，源代码位于
[gubaiovo/PCL 的 fix-network 分支](https://github.com/gubaiovo/PCL/tree/fix-network)。

- 移除 Minecraft 下载、版本修改与整合包安装的开源版限制。
- 禁用启动器自动更新、更新检查与程序替换，测试构建需手动下载。
- Minecraft 本体、资源与 Java 运行时使用 Mojang 官方地址；Forge、NeoForge、Fabric、LiteLoader、OptiFine 使用各自的官方地址；模组与整合包使用 Modrinth、CurseForge 官方 API 和 CDN。
- 下载失败会报告官方源的错误，不再回退到 BMCLAPI、MCIM 等镜像。官方已撤下的文件（例如部分 OptiFine 预览版）可能无法重新下载。
- 内嵌 Forge 安装辅助程序也改用官方 Maven，并在 CI 中从可审查源码重新编译。

在本仓库的 [Actions](https://github.com/gubaiovo/PCL/actions/workflows/build.yml) 中选择
`Build` 工作流和 `fix-network` 分支，执行 `Run workflow`；向此分支推送代码也会触发构建。
完成后下载名称以 `PCL-fix-network-Release-` 开头的构建产物，解压后运行其中的启动器。
工作流同时提供 Debug 产物，包含完整输出目录以保留运行依赖。

微软登录仍需自行注册的 Client ID；CurseForge 官方 API 仍需有效 API Key。
可在构建仓库设置 `CLIENT_ID`、`CURSEFORGE_API_KEY` Secrets，或在运行环境设置
`PCL_MS_CLIENT_ID`、`PCL_CURSEFORGE_API_KEY`。Modrinth 的公开下载接口无需这两项配置。

本地网络策略检查：`python .github/scripts/check_network_policy.py`。
构建需要 Windows、支持 C# 13 的 MSBuild/.NET SDK 和 .NET Framework 4.8 开发工具；
重建 Forge 辅助程序的方法见 [tools/forge-installer](tools/forge-installer/README.md)。

原项目说明：

# Plain Craft Launcher

[![Stars](https://img.shields.io/github/stars/Meloong-Git/PCL?style=flat&logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEiIHdpZHRoPSIxNiIgaGVpZ2h0PSIxNiI+PHBhdGggZD0iTTggLjI1YS43NS43NSAwIDAgMSAuNjczLjQxOGwxLjg4MiAzLjgxNSA0LjIxLjYxMmEuNzUuNzUgMCAwIDEgLjQxNiAxLjI3OWwtMy4wNDYgMi45Ny43MTkgNC4xOTJhLjc1MS43NTEgMCAwIDEtMS4wODguNzkxTDggMTIuMzQ3bC0zLjc2NiAxLjk4YS43NS43NSAwIDAgMS0xLjA4OC0uNzlsLjcyLTQuMTk0TC44MTggNi4zNzRhLjc1Ljc1IDAgMCAxIC40MTYtMS4yOGw0LjIxLS42MTFMNy4zMjcuNjY4QS43NS43NSAwIDAgMSA4IC4yNVoiIGZpbGw9IiNlYWM1NGYiLz48L3N2Zz4=&logoSize=auto&label=Stars&labelColor=444444&color=eac54f)](https://github.com/Meloong-Git/PCL/)
[![Issues](https://img.shields.io/github/issues/Meloong-Git/PCL?style=flat&label=Issues&labelColor=444444&color=1F883D)](https://github.com/Meloong-Git/PCL/issues)
[![哔哩哔哩](https://img.shields.io/badge/动态-BiliBili-00A4DB?style=flat&labelColor=444444&logoSize=auto)](https://space.bilibili.com/11343203/dynamic)
[![爱发电](https://img.shields.io/badge/赞助-爱发电-946ce6?style=flat&labelColor=444444&logoSize=auto)](https://meloong.com/afd/a/LTCat)

包含 PCL 的大部分源代码，包括 UI 库、动画模块、下载模块、Minecraft 启动模块等！<br/>
代码并不是实时更新的，它会在每次 PCL 发布更新时同步一次。<br/>
非常感谢社区的大力支持！

<br/>

### 相关资源
- [PCL 下载](https://meloong.com/pcl)：下载可免费使用的正式版 PCL！
- [PCL 功能投票](https://github.com/Meloong-Git/PCL/discussions/2)：来参加投票吧，开发者会优先处理票数较高的项目！
- [帮助文档库](https://github.com/LTCatt/PCL2Help)：PCL 内置帮助文档的存储库
