#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using TvLibrary.Interfaces;
using TvLibrary.Interfaces.Analyzer;
using TvLibrary.Channels;
using TvLibrary.Implementations.DVB.Structures;

namespace TvLibrary.Implementations.DVB
{
  /// <summary>
  /// Class which implements scanning for tv/radio channels for ATSC BDA cards
  /// </summary>
  public class ATSCScanning : DvbBaseScanning
  {
    /// <summary>
    /// ATSC service types - see A/53 part 1
    /// </summary>
    protected enum AtscServiceType
    {
      /// <summary>
      /// Analog Television (See A/65 [9])
      /// </summary>
      AnalogTelevision = 0x01,
      /// <summary>
      /// ATSC Digital Television (See A/53-3 [2])
      /// </summary>
      DigitalTelevision = 0x02,
      /// <summary>
      /// ATSC Audio (See A/53-3 [2])
      /// </summary>
      Audio = 0x03
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ATSCScanning"/> class.
    /// </summary>
    /// <param name="card">The card.</param>
    public ATSCScanning(TvCardDvbBase card)
      : base(card)
    {
      _enableWaitForVCT = true;
    }

    /// <summary>
    /// Creates the new channel.
    /// </summary>
    /// <param name="channel">The high level tuning detail.</param>
    /// <param name="info">The subchannel detail.</param>
    /// <returns>The new channel.</returns>
    protected override IChannel CreateNewChannel(IChannel channel, ChannelInfo info)
    {
      ATSCChannel tuningChannel = (ATSCChannel)channel;
      ATSCChannel atscChannel = new ATSCChannel();
      atscChannel.Name = info.service_name;
      if (info.LCN == 0 || info.LCN == 10000)
      {
        // logical channel number not found/assigned
        if (info.majorChannel > 0 && info.minorChannel > 0)
        {
          // ATSC two part channel number available. See ATSC A/65 section
          // 6.3.1 and 6.3.2, notes on major_channel_number and
          // minor_channel_number. Since MP currently doesn't support
          // storing or zapping with such numbers, we use an
          // easy-to-remember convention...
          info.LCN = (info.majorChannel * 1000) + info.minorChannel;
        }
        else
        {
          info.LCN = tuningChannel.PhysicalChannel * 1000 + info.serviceID;
        }
      }
      atscChannel.Provider = info.service_provider_name;
      atscChannel.ModulationType = tuningChannel.ModulationType;
      atscChannel.Frequency = tuningChannel.Frequency;
      atscChannel.PhysicalChannel = tuningChannel.PhysicalChannel;
      atscChannel.MajorChannel = info.majorChannel;
      atscChannel.MinorChannel = info.minorChannel;
      atscChannel.IsTv = IsTvService(info.serviceType);
      atscChannel.IsRadio = IsRadioService(info.serviceType);
      atscChannel.NetworkId = info.networkID;
      atscChannel.ServiceId = info.serviceID;
      atscChannel.TransportId = info.transportStreamID;
      atscChannel.PmtPid = info.network_pmt_PID;
      atscChannel.FreeToAir = !info.scrambled;
      Log.Log.Write("atsc:Found: {0}", atscChannel);
      return atscChannel;
    }

    protected override void SetNameForUnknownChannel(IChannel channel, ChannelInfo info)
    {
      ATSCChannel atscChannel = channel as ATSCChannel;
      if (channel == null)
      {
        return;
      }

      info.service_name = String.Format("Unknown {0}-{1}", atscChannel.PhysicalChannel, info.serviceID);
      Log.Log.Info("ATSCScanning: channel name is not available, now assigned \"{0}\"", info.service_name);
    }

    protected override bool IsRadioService(int serviceType)
    {
      return serviceType == (int)AtscServiceType.Audio;
    }

    protected override bool IsTvService(int serviceType)
    {
      return serviceType == (int)AtscServiceType.AnalogTelevision ||
             serviceType == (int)AtscServiceType.DigitalTelevision;
    }
  }
}