using UnityEngine;
using System.Collections;

public class UnderwaterBubbleParticle : SPGameEngineParticle, GenericPooledObject {
	
	public static UnderwaterBubbleParticle cons(Vector2 start, Vector2 end) {
		return ObjectPool.inst().generic_depool<UnderwaterBubbleParticle>().i_cons(start, end);
	}
	
	private SPSprite _img;
	public void depool() {
		_img = SPSprite.cons_sprite_texkey_texrect(RTex.PARTICLES_SPRITESHEET,FileCache.inst().get_texrect(RTex.PARTICLES_SPRITESHEET,"particle_bubble.png"));
		_img.set_name("UnderwaterBubbleParticle");
	}
	public void repool() {
		_img.repool();
		_img = null;
	}
	
	private float _anim_t, _wave_t;
	private SPRange _scale, _opacity;
	private Vector2 _start, _end;
	public UnderwaterBubbleParticle i_cons(Vector2 start, Vector2 end) {
		_img.set_manual_sort_z_order(GameAnchorZ.BGWater_FX);
		_anim_t = 0;
		_wave_t = SPUtil.float_random(-3.14f, 3.14f);
		_start = start;
		_end = end;
		_scale = new SPRange() { _min = 1.2f, _max = 1.8f };
		_opacity = new SPRange() { _min = 0.8f, _max = 0.1f };
		this.i_update(null);
		return this;
	}
	public override void i_update(GameEngineScene g) {
		_anim_t += SPUtil.dt_scale_get() * 0.05f;
		_wave_t += SPUtil.dt_scale_get() * 0.05f;
		float wave_offset = SPUtil.lerp(0,60,_anim_t) * Mathf.Sin(_wave_t);
		_img.set_u_pos(SPUtil.vec_add(Vector2.Lerp(_start,_end, _anim_t),new Vector2(wave_offset,0)));
		_img.set_opacity(SPUtil.lerp(_opacity._min,_opacity._max,_anim_t));
		_img.set_scale(SPUtil.lerp(_scale._min,_scale._max,_anim_t));
	}
	public override bool should_remove(GameEngineScene g) {
		return _anim_t >= 1;
	}
	public override void do_remove(GameEngineScene g) {
		ObjectPool.inst().generic_repool<UnderwaterBubbleParticle>(this);
	}
	public override void add_to_parent(SPNode parent) {
		parent.add_child(_img);
	}
	
	public static void proc_multiple_bubbles(GameEngineScene g) {
		UnderwaterBubbleParticle.proc_bubble(g);
		for (int i = 0; i < 12; i++) {
			g._delayed_actions.enqueue_action(new DelayedAction() {
				_time_left = SPUtil.float_random(0,12),
				_callback = __delayed_proc_bubble
			});
		}
	}
	private static void __delayed_proc_bubble(GameEngineScene g) {
		Vector2 pos = new Vector2(
			g._player.get_center().x + SPUtil.float_random(-25,25),
			g._player.get_center().y + SPUtil.float_random(-25,25)
		);
		g.add_particle(UnderwaterBubbleParticle.cons(
			pos, SPUtil.vec_add(pos, new Vector2(0,(100+SPUtil.float_random(-50,50))*2)
		)));
	}
	private static void __delayed_proc_bubble2(GameEngineScene g) {
		Vector2 pos = new Vector2(
			g._player.get_center().x + SPUtil.float_random(-25,25),
			g._player.get_center().y + SPUtil.float_random(-25,25)
		);
		g.add_particle(UnderwaterBubbleParticle.cons(
			pos, SPUtil.vec_add(pos, new Vector2(0,(100+SPUtil.float_random(-50,50))*2)
		)));
	}
	public static void proc_multiple_bubbles2(GameEngineScene g) {
		UnderwaterBubbleParticle.proc_bubble(g);
		for (int i = 0; i < 25; i++) {
			g._delayed_actions.enqueue_action(new DelayedAction() {
				_time_left = SPUtil.float_random(10,35),
				_callback = __delayed_proc_bubble2
			});
		}
	}
	public static void proc_bubble(GameEngineScene g) {
		Vector2 player_pos = SPUtil.vec_add(g._player.get_center(),new Vector2(SPUtil.float_random(-15,15),SPUtil.float_random(-15,15)));
		g.add_particle(UnderwaterBubbleParticle.cons(
			player_pos,
			SPUtil.vec_add(player_pos,new Vector2(0,(100+SPUtil.float_random(-80,10))*2))	
		));
	}
	
	
}
