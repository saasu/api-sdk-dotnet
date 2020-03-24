Saasu.API.Sdk for .Net

A Client SDK for working with the Saasu API.

Please see https://api.saasu.com/ for help information.

The following application settings configuration section and corresponding values are required in your application configuration file:

    <configSections>
        <sectionGroup name="applicationSettings" type="System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089">
            <section name="Saasu.API.Client.Config" type="System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
        </sectionGroup>
    </configSections>

  <applicationSettings>
    <Saasu.API.Client.Config>
      <setting name="IgnoreSSLErrors" serializeAs="String">
        <value>false</value>
      </setting>
      <setting name="BaseUri" serializeAs="String">
        <value>https://api.saasu.com/</value>
      </setting>
      <setting name="WsAccessKey" serializeAs="String">
        <value>{YOUR-WS-ACCESS-KEY}</value>
      </setting>
      <setting name="FileUid" serializeAs="String">
        <value>0</value>
      </setting>
    </Saasu.API.Client.Config>
  </applicationSettings>
