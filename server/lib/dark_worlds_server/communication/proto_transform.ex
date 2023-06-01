defmodule DarkWorldsServer.Communication.ProtoTransform do
  alias DarkWorldsServer.Communication.Proto.ClientAction, as: ProtoAction
  alias DarkWorldsServer.Communication.Proto.Player, as: ProtoPlayer
  alias DarkWorldsServer.Communication.Proto.Position, as: ProtoPosition
  alias DarkWorldsServer.Communication.Proto.RelativePosition, as: ProtoRelativePosition
  alias DarkWorldsServer.Engine.ActionOk, as: EngineAction
  alias DarkWorldsServer.Engine.Player, as: EnginePlayer
  alias DarkWorldsServer.Engine.Position, as: EnginePosition
  alias DarkWorldsServer.Engine.RelativePosition, as: EngineRelativePosition

  @behaviour Protobuf.TransformModule

  @impl Protobuf.TransformModule
  def encode(%EnginePosition{} = position, ProtoPosition) do
    %{x: x, y: y} = position
    %ProtoPosition{x: x, y: y}
  end

  def encode(%EnginePlayer{} = player, ProtoPlayer) do
    %{id: id, health: health, position: position, action: action, aoe_position: aoe_position} = player

    %ProtoPlayer{
      id: id,
      health: health,
      position: position,
      action: player_action_encode(action),
      aoe_position: aoe_position
    }
  end

  def encode(%EngineAction{action: :move, value: direction}, ProtoAction) do
    %ProtoAction{action: :MOVE, direction: direction_encode(direction)}
  end

  def encode(%EngineAction{action: :attack, value: direction}, ProtoAction) do
    %ProtoAction{action: :ATTACK, direction: direction_encode(direction)}
  end

  def encode(%EngineAction{action: :attack_aoe, value: position}, ProtoAction) do
    %ProtoAction{action: :ATTACK_AOE, position: position}
  end

  @impl Protobuf.TransformModule
  def decode(%ProtoPosition{} = position, ProtoPosition) do
    %{x: x, y: y} = position
    %EnginePosition{x: x, y: y}
  end

  @impl Protobuf.TransformModule
  def decode(%ProtoRelativePosition{} = position, ProtoRelativePosition) do
    %{x: x, y: y} = position
    %EngineRelativePosition{x: x, y: y}
  end

  def decode(%ProtoPlayer{} = player, ProtoPlayer) do
    %{
      id: id,
      health: health,
      position: position,
      last_melee_attack: attack,
      status: status,
      action: action,
      aoe_position: aoe_position
    } = player

    %EnginePlayer{
      id: id,
      health: health,
      position: position,
      last_melee_attack: attack,
      status: status,
      action: player_action_decode(action),
      aoe_position: aoe_position
    }
  end

  def decode(%ProtoAction{action: :MOVE_WITH_JOYSTICK, move_delta: %{x: x, y: y}}, ProtoAction) do
    %EngineAction{action: :move_with_joystick, value: %{x: x, y: y}}
  end

  def decode(%ProtoAction{action: :MOVE, direction: direction}, ProtoAction) do
    %EngineAction{action: :move, value: direction_decode(direction)}
  end

  def decode(%ProtoAction{action: :ATTACK, direction: direction}, ProtoAction) do
    %EngineAction{action: :attack, value: direction_decode(direction)}
  end

  def decode(%ProtoAction{action: :ATTACK_AOE, position: position}, ProtoAction) do
    %EngineAction{action: :attack_aoe, value: position}
  end

  def decode(%ProtoAction{action: :ADD_BOT}, ProtoAction) do
    %EngineAction{action: :add_bot, value: nil}
  end

  def decode(%struct{} = msg, struct) do
    Map.from_struct(msg)
  end

  ###############################
  # Helpers for transformations #
  ###############################
  defp direction_encode(:up), do: :UP
  defp direction_encode(:down), do: :DOWN
  defp direction_encode(:left), do: :LEFT
  defp direction_encode(:right), do: :RIGHT

  defp direction_decode(:UP), do: :up
  defp direction_decode(:DOWN), do: :down
  defp direction_decode(:LEFT), do: :left
  defp direction_decode(:RIGHT), do: :right

  defp player_action_encode(:attacking), do: :ATTACKING
  defp player_action_encode(:nothing), do: :NOTHING
  defp player_action_encode(:attackingaoe), do: :ATTACKING_AOE

  defp player_action_decode(:ATTACKING), do: :attacking
  defp player_action_decode(:NOTHING), do: :nothing
  defp player_action_decode(:ATTACKING_AOE), do: :attackingaoe
end
