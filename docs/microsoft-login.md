# 微软登录配置

Client ID 是微软分配给应用程序的公开编号，不是游戏 UUID、微软账号或密码。
PCL 开源代码不包含官方版本的 Client ID；此 fork 需要使用自己注册的应用。
没有有效应用编号时，重新打包 EXE 无法使微软接受登录请求。

## 配置已注册的应用

打开启动器的 `设置 → 其他 → 微软登录`，填写应用程序（客户端）ID，然后重新登录。
这是公开的应用标识，此处不填写 client secret。

也可以在启动器运行环境设置 `PCL_MS_CLIENT_ID`，或在 GitHub 仓库的
`Settings → Secrets and variables → Actions → Variables` 中添加 `PCL_MS_CLIENT_ID` 并重新构建。
同名 Secret 和旧的 `CLIENT_ID` Variable / Secret 也受支持。

运行环境变量优先于启动器设置，启动器设置优先于构建中嵌入的值。
空白环境变量不会覆盖已有配置。一次设备授权及其令牌轮询使用同一个 Client ID。

## 注册自己的应用

1. 按照 [微软官方应用注册说明](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app)，
   使用具备应用注册权限的 Microsoft Entra 租户创建应用。
2. 应用账户类型需要支持个人 Microsoft 账户，因为本启动器使用 `/consumers` 登录端点。
3. 按照 [微软设备授权流程说明](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-device-code)
   和 [公共客户端配置说明](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-desktop-app-registration)，
   为桌面公共客户端启用设备代码流程。
4. 从应用“概述”复制“应用程序（客户端）ID”，按上文配置到启动器或构建仓库。

Microsoft 账户授权、Xbox 登录和 Minecraft Services 登录是不同的步骤。
完成应用注册不等于获得 Minecraft Services 的访问许可；如果后续收到
`Invalid app registration`，需要处理该应用的 Minecraft API 访问审批。
启动器会区分此错误与普通网络错误，并保留正版购买和玩家档案检查。

## 构建验证的范围

CI 验证配置优先级、Client ID 缺失/格式错误、表单编码以及登录请求参数，并从单 EXE 目录检查
禁用更新命令。真实 Microsoft 账户授权和 Minecraft Services 登录需要配置有效应用并由用户完成浏览器登录，
不能仅凭编译或离线测试声称已经通过。
